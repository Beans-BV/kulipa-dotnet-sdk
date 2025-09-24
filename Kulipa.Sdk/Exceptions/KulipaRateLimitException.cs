using System.Net;

namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Exception thrown when rate limit is exceeded (429).
    /// </summary>
    public class KulipaRateLimitException : KulipaApiException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaRateLimitException" /> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="remainingRequests">The number of remaining requests in the current rate limit window.</param>
        /// <param name="resetTime">The UTC time when the rate limit window resets.</param>
        /// <param name="retryAfterSeconds">The number of seconds to wait before retrying the request.</param>
        /// <param name="requestId">The optional request ID for tracking purposes.</param>
        public KulipaRateLimitException(
            string message,
            int remainingRequests,
            DateTime resetTime,
            int retryAfterSeconds,
            string? requestId = null)
            : base(message, null, requestId, HttpStatusCode.TooManyRequests)
        {
            RemainingRequests = remainingRequests;
            ResetTime = resetTime;
            RetryAfterSeconds = retryAfterSeconds;
        }

        /// <summary>
        ///     Gets the number of remaining requests in the current rate limit window.
        /// </summary>
        public int RemainingRequests { get; }

        /// <summary>
        ///     Gets the UTC time when the rate limit window resets.
        /// </summary>
        public DateTime ResetTime { get; }

        /// <summary>
        ///     Gets the number of seconds to wait before retrying the request.
        /// </summary>
        public int RetryAfterSeconds { get; }
    }
}