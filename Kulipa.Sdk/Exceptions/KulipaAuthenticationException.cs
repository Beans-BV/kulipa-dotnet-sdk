using System.Net;

namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Exception thrown for authentication failures (401).
    /// </summary>
    public class KulipaAuthenticationException : KulipaApiException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaAuthenticationException" /> class with a specified error
        ///     message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="requestId">The request ID associated with the error.</param>
        public KulipaAuthenticationException(string message, string? requestId = null)
            : base(message, null, requestId, HttpStatusCode.Unauthorized)
        {
        }
    }
}