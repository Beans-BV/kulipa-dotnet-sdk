using System.Net;

namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Base exception for all Kulipa SDK exceptions.
    /// </summary>
    public class KulipaException : Exception
    {
        public KulipaException(string message, string? requestId = null)
            : base(message)
        {
            RequestId = requestId;
        }

        public KulipaException(string message, Exception innerException, string? requestId = null)
            : base(message, innerException)
        {
            RequestId = requestId;
        }

        public string? RequestId { get; }
    }

    /// <summary>
    ///     Exception thrown when API returns an error response.
    /// </summary>
    public class KulipaApiException : KulipaException
    {
        public KulipaApiException(string message, string? responseContent = null, string? requestId = null, HttpStatusCode? statusCode = null)
            : base(message, requestId)
        {
            ResponseContent = responseContent;
            StatusCode = statusCode;
        }

        public string? ResponseContent { get; }
        public HttpStatusCode? StatusCode { get; }
    }

    /// <summary>
    ///     Exception thrown for authentication failures (401).
    /// </summary>
    public class KulipaAuthenticationException : KulipaApiException
    {
        public KulipaAuthenticationException(string message, string? requestId = null)
            : base(message, null, requestId, HttpStatusCode.Unauthorized)
        {
        }
    }

    /// <summary>
    ///     Exception thrown for authorization failures (403).
    /// </summary>
    public class KulipaAuthorizationException : KulipaApiException
    {
        public KulipaAuthorizationException(string message, string? requestId = null)
            : base(message, null, requestId, HttpStatusCode.Forbidden)
        {
        }
    }

    /// <summary>
    ///     Exception thrown for validation errors (400).
    /// </summary>
    public class KulipaValidationException : KulipaApiException
    {
        public KulipaValidationException(string message, string? responseContent = null, string? requestId = null)
            : base(message, responseContent, requestId, HttpStatusCode.BadRequest)
        {
        }
    }

    /// <summary>
    ///     Exception thrown when rate limit is exceeded (429).
    /// </summary>
    public class KulipaRateLimitException : KulipaApiException
    {
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

        public int RemainingRequests { get; }
        public DateTime ResetTime { get; }
        public int RetryAfterSeconds { get; }
    }

    /// <summary>
    ///     Exception thrown for server errors (500, 502, 503, 504).
    /// </summary>
    public class KulipaServerException : KulipaApiException
    {
        public KulipaServerException(string message, string? requestId = null, HttpStatusCode? statusCode = null)
            : base(message, null, requestId, statusCode)
        {
        }
    }
}