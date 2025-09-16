using System.Net;
using Kulipa.Sdk.Exceptions;

namespace Kulipa.Sdk.Services.Http
{
    /// <summary>
    ///     Handles rate limiting for API requests.
    /// </summary>
    public class RateLimitHandler : DelegatingHandler
    {
        private readonly ILogger<RateLimitHandler> _logger;
        private readonly SemaphoreSlim _semaphore;
        private readonly object _lockObject = new();
        private int _remainingRequests = 300;
        private DateTime _resetTime = DateTime.UtcNow.AddMinutes(1);

        public RateLimitHandler(ILogger<RateLimitHandler> logger)
        {
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                // Check if we should wait before making the request with thread-safe reads
                int currentRemaining;
                DateTime currentResetTime;
                lock (_lockObject)
                {
                    currentRemaining = _remainingRequests;
                    currentResetTime = _resetTime;
                }

                if (currentRemaining <= 0 && DateTime.UtcNow < currentResetTime)
                {
                    var waitTime = currentResetTime - DateTime.UtcNow;
                    _logger.LogWarning("Rate limit reached. Waiting {WaitTime} seconds", waitTime.TotalSeconds);
                    await Task.Delay(waitTime, cancellationToken);
                }

                var response = await base.SendAsync(request, cancellationToken);

                // Update rate limit information from headers
                UpdateRateLimitInfo(response);

                // Handle 429 responses
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = GetRetryAfterSeconds(response);

                    // Get current values safely for exception
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
            finally
            {
                _semaphore.Release();
            }
        }

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

        private int GetRetryAfterSeconds(HttpResponseMessage response)
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