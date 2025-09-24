namespace Kulipa.Sdk.Exceptions
{
    /// <summary>
    ///     Base exception for all Kulipa SDK exceptions.
    /// </summary>
    public class KulipaException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="requestId">The optional request ID associated with the error.</param>
        public KulipaException(string message, string? requestId = null)
            : base(message)
        {
            RequestId = requestId;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaException" /> class with a specified error message and inner
        ///     exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <param name="requestId">The optional request ID associated with the error.</param>
        public KulipaException(string message, Exception innerException, string? requestId = null)
            : base(message, innerException)
        {
            RequestId = requestId;
        }


        /// <summary>
        ///     Gets the request ID associated with this exception, if available.
        /// </summary>
        public string? RequestId { get; }
    }
}