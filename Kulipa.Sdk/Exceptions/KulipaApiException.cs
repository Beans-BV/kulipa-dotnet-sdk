using System.Net;

namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Exception thrown when API returns an error response.
    /// </summary>
    public class KulipaApiException : KulipaException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaApiException" /> class with specified details.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="responseContent">The response content from the API.</param>
        /// <param name="requestId">The request ID associated with the error.</param>
        /// <param name="statusCode">The HTTP status code returned by the API.</param>
        public KulipaApiException(string message, string? responseContent = null, string? requestId = null,
            HttpStatusCode? statusCode = null)
            : base(message, requestId)
        {
            ResponseContent = responseContent;
            StatusCode = statusCode;
        }


        /// <summary>
        ///     Gets the response content from the API, if available.
        /// </summary>
        public string? ResponseContent { get; }

        /// <summary>
        ///     Gets the HTTP status code returned by the API, if available.
        /// </summary>
        public HttpStatusCode? StatusCode { get; }
    }
}