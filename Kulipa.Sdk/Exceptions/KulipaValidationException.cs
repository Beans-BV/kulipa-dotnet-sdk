using System.Net;

namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Exception thrown for validation errors (400).
    /// </summary>
    public class KulipaValidationException : KulipaApiException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaValidationException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="responseContent">The response content from the API.</param>
        /// <param name="requestId">The request ID associated with the error.</param>
        public KulipaValidationException(string message, string? responseContent = null, string? requestId = null)
            : base(message, responseContent, requestId, HttpStatusCode.BadRequest)
        {
        }
    }
}