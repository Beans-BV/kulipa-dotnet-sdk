using System.Net;
using Kulipa.Sdk.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kulipa.Sdk.Services.Http
{
    /// <summary>
    ///     HTTP message handler that manages rate limiting for Kulipa API requests.
    ///     Tracks remaining requests and reset times, automatically waits when limits are exceeded,
    ///     and throws <see cref="KulipaRateLimitException" /> on 429 responses.
    /// </summary>
    public class RateLimitHandler : DelegatingHandler
    {
        private readonly object _lockObject = new();
        private readonly ILogger<RateLimitHandler> _logger;
        private int _remainingRequests = 300;
        private DateTime _resetTime = DateTime.UtcNow.AddMinutes(1);

        /// <summary>
        ///     Initializes a new instance of the <see cref="RateLimitHandler" /> class.
        /// </summary>
        /// <param name="logger">The logger instance for rate limit events.</param>
        public RateLimitHandler(ILogger<RateLimitHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Sends an HTTP request asynchronously with rate limit management.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The HTTP response message.</returns>
        /// <exception cref="KulipaRateLimitException">Thrown when the API returns a 429 Too Many Requests status.</exception>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Check rate limit (fast, non-blocking)
            bool shouldWait;
            DateTime waitUntil;
            lock (_lockObject)
            {
                shouldWait = _remainingRequests <= 0 && DateTime.UtcNow < _resetTime;
                waitUntil = _resetTime;
            }

            if (shouldWait)
            {
                var waitTime = waitUntil - DateTime.UtcNow;
                _logger.LogWarning("Rate limit reached. Waiting {WaitTime} seconds", waitTime.TotalSeconds);
                await Task.Delay(waitTime, cancellationToken);
            }

            // Execute request concurrently
            var response = await base.SendAsync(request, cancellationToken);

            // Update state (protected by lock, but non-blocking)
            UpdateRateLimitInfo(response);

            // Handle 429 responses
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var retryAfter = GetRetryAfterSeconds(response);

                // Get current values safely for exception
                int currentRemaining;
                DateTime currentResetTime;
                lock (_lockObject)
                {
                    currentRemaining = _remainingRequests;
                    currentResetTime = _resetTime;
                }

                throw new KulipaRateLimitException(
                    "Rate limit exceeded",
                    currentRemaining,
                    currentResetTime,
                    retryAfter);
            }

            return response;
        }

        /// <summary>
        ///     Updates internal rate limit state based on response headers.
        /// </summary>
        /// <param name="response">The HTTP response containing rate limit headers.</param>
        private void UpdateRateLimitInfo(HttpResponseMessage response)
        {
            int currentRemaining;
            DateTime currentResetTime;

            lock (_lockObject)
            {
                currentRemaining = _remainingRequests;
                currentResetTime = _resetTime;

                if (response.Headers.TryGetValues("x-ratelimit-remaining", out var remaining))
                {
                    if (int.TryParse(remaining.FirstOrDefault(), out var remainingCount))
                    {
                        _remainingRequests = remainingCount;
                        currentRemaining = remainingCount;
                    }
                }

                if (response.Headers.TryGetValues("x-ratelimit-reset", out var reset))
                {
                    if (int.TryParse(reset.FirstOrDefault(), out var resetSeconds))
                    {
                        _resetTime = DateTime.UtcNow.AddSeconds(resetSeconds);
                        currentResetTime = _resetTime;
                    }
                }
            }

            if (response.Headers.TryGetValues("x-request-id", out var requestId))
            {
                _logger.LogDebug("Request ID: {RequestId}, Remaining: {Remaining}, Reset: {Reset}",
                    requestId.FirstOrDefault(),
                    currentRemaining,
                    currentResetTime);
            }
        }

        /// <summary>
        ///     Extracts the retry-after duration from response headers.
        /// </summary>
        /// <param name="response">The HTTP response containing the Retry-After header.</param>
        /// <returns>The number of seconds to wait before retrying, defaults to 60 if header is missing or invalid.</returns>
        private static int GetRetryAfterSeconds(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Retry-After", out var retryAfter))
            {
                if (int.TryParse(retryAfter.FirstOrDefault(), out var seconds))
                {
                    return seconds;
                }
            }

            return 60; // Default to 60 seconds
        }
    }
}