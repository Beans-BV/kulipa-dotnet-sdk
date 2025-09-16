namespace Kulipa.Sdk.Models.Webhooks
{
    /// <summary>
    ///     Represents the result of a webhook signature verification.
    /// </summary>
    public class WebhookVerificationResult
    {
        /// <summary>
        ///     Gets or sets whether the webhook signature is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        ///     Gets or sets the error message if verification failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        ///     Gets or sets the verification failure reason.
        /// </summary>
        public VerificationFailureReason? FailureReason { get; set; }

        /// <summary>
        ///     Creates a successful verification result.
        /// </summary>
        public static WebhookVerificationResult Success()
        {
            return new WebhookVerificationResult { IsValid = true };
        }

        /// <summary>
        ///     Creates a failed verification result.
        /// </summary>
        public static WebhookVerificationResult Failure(string errorMessage, VerificationFailureReason reason)
        {
            return new WebhookVerificationResult
            {
                IsValid = false,
                ErrorMessage = errorMessage,
                FailureReason = reason
            };
        }
    }

    /// <summary>
    ///     Reasons for webhook verification failure.
    /// </summary>
    public enum VerificationFailureReason
    {
        /// <summary>
        ///     Required headers are missing.
        /// </summary>
        MissingHeaders,

        /// <summary>
        ///     The signature format is invalid.
        /// </summary>
        InvalidSignatureFormat,

        /// <summary>
        ///     The timestamp format is invalid.
        /// </summary>
        InvalidTimestampFormat,

        /// <summary>
        ///     The timestamp is too old (replay attack prevention).
        /// </summary>
        TimestampTooOld,

        /// <summary>
        ///     The timestamp is in the future.
        /// </summary>
        TimestampInFuture,

        /// <summary>
        ///     Failed to fetch the public key.
        /// </summary>
        KeyFetchFailed,

        /// <summary>
        ///     The public key format is invalid.
        /// </summary>
        InvalidPublicKeyFormat,

        /// <summary>
        ///     The signature verification failed.
        /// </summary>
        SignatureVerificationFailed,

        /// <summary>
        ///     The algorithm is not supported.
        /// </summary>
        UnsupportedAlgorithm
    }
}