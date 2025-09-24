using System.Net;

namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Exception thrown for server errors (500, 502, 503, 504).
    /// </summary>
    public class KulipaServerException : KulipaApiException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaServerException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="requestId">The request ID associated with the error.</param>
        /// <param name="statusCode">The HTTP status code returned by the API.</param>
        public KulipaServerException(string message, string? requestId = null, HttpStatusCode? statusCode = null)
            : base(message, null, requestId, statusCode)
        {
        }
    }
}