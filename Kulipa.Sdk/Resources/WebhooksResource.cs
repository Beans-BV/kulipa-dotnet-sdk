using Kulipa.Sdk.Models.Webhooks;
using Kulipa.Sdk.Webhooks;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Provides webhook-related operations.
    /// </summary>
    public class WebhooksResource : IWebhooksResource
    {
        private readonly IPublicKeyCache _keyCache;
        private readonly IWebhookVerifier _verifier;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebhooksResource" /> class.
        /// </summary>
        public WebhooksResource(
            IWebhookVerifier verifier,
            IPublicKeyCache keyCache)
        {
            _verifier = verifier ?? throw new ArgumentNullException(nameof(verifier));
            _keyCache = keyCache ?? throw new ArgumentNullException(nameof(keyCache));
        }

        /// <summary>
        ///     Verifies a webhook signature.
        /// </summary>
        /// <param name="signature">The signature from x-kulipa-signature header.</param>
        /// <param name="timestamp">The timestamp from x-kulipa-signature-ts header.</param>
        /// <param name="keyId">The key ID from x-kulipa-key-id header.</param>
        /// <param name="rawBody">The raw request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The verification result.</returns>
        public Task<WebhookVerificationResult> VerifySignatureAsync(
            string signature,
            string timestamp,
            string keyId,
            string rawBody,
            CancellationToken cancellationToken = default)
        {
            return _verifier.VerifySignatureAsync(signature, timestamp, keyId, rawBody, cancellationToken);
        }

        /// <summary>
        ///     Verifies a webhook using headers and body.
        /// </summary>
        /// <param name="headers">The request headers.</param>
        /// <param name="rawBody">The raw request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The verification result.</returns>
        public Task<WebhookVerificationResult> VerifyWebhookAsync(
            IDictionary<string, string> headers,
            string rawBody,
            CancellationToken cancellationToken = default)
        {
            return _verifier.VerifyWebhookAsync(headers, rawBody, cancellationToken);
        }

        /// <summary>
        ///     Verifies a webhook using headers and body.
        /// </summary>
        /// <param name="headers">The request headers.</param>
        /// <param name="rawBody">The raw request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The verification result.</returns>
        public Task<WebhookVerificationResult> VerifyWebhookAsync(
            IEnumerable<KeyValuePair<string, string>> headers,
            string rawBody,
            CancellationToken cancellationToken = default)
        {
            return _verifier.VerifyWebhookAsync(headers, rawBody, cancellationToken);
        }

        /// <summary>
        ///     Invalidates a cached public key.
        /// </summary>
        /// <param name="keyId">The key ID to invalidate.</param>
        public void InvalidateCachedKey(string keyId)
        {
            _keyCache.InvalidateKey(keyId);
        }

        /// <summary>
        ///     Clears all cached public keys.
        /// </summary>
        public void ClearKeyCache()
        {
            _keyCache.Clear();
        }
    }
}