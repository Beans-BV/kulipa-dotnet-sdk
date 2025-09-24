using System.Net;

namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Exception thrown for authorization failures (403).
    /// </summary>
    public class KulipaAuthorizationException : KulipaApiException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaAuthorizationException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="requestId">The request ID associated with the error.</param>
        public KulipaAuthorizationException(string message, string? requestId = null)
            : base(message, null, requestId, HttpStatusCode.Forbidden)
        {
        }
    }
}