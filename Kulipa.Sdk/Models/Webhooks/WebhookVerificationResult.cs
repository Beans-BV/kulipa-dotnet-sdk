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
}