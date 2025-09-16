using Kulipa.Sdk.Models.Webhooks;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Defines the contract for webhook-related operations.
    /// </summary>
    public interface IWebhooksResource
    {
        /// <summary>
        ///     Verifies a webhook signature using the provided components.
        /// </summary>
        /// <param name="signature">The signature from x-kulipa-signature header.</param>
        /// <param name="timestamp">The timestamp from x-kulipa-signature-ts header.</param>
        /// <param name="keyId">The key ID from x-kulipa-key-id header.</param>
        /// <param name="rawBody">The raw request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The verification result.</returns>
        Task<WebhookVerificationResult> VerifySignatureAsync(
            string signature,
            string timestamp,
            string keyId,
            string rawBody,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Verifies a webhook using headers and body.
        /// </summary>
        /// <param name="headers">The request headers (case-insensitive).</param>
        /// <param name="rawBody">The raw request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The verification result.</returns>
        Task<WebhookVerificationResult> VerifyWebhookAsync(
            IDictionary<string, string> headers,
            string rawBody,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Verifies a webhook using headers and body.
        /// </summary>
        /// <param name="headers">The request headers.</param>
        /// <param name="rawBody">The raw request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The verification result.</returns>
        Task<WebhookVerificationResult> VerifyWebhookAsync(
            IEnumerable<KeyValuePair<string, string>> headers,
            string rawBody,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Invalidates a cached public key.
        /// </summary>
        /// <param name="keyId">The key ID to invalidate.</param>
        void InvalidateCachedKey(string keyId);

        /// <summary>
        ///     Clears all cached public keys.
        /// </summary>
        void ClearKeyCache();
    }
}