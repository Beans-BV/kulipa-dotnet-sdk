using Kulipa.Sdk.Models.Webhooks;

namespace Kulipa.Sdk.Webhooks
{
    /// <summary>
    ///     Provides caching for webhook public keys.
    /// </summary>
    public interface IPublicKeyCache
    {
        /// <summary>
        ///     Gets a public key by ID, fetching from API if not cached.
        /// </summary>
        /// <param name="keyId">The key identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The webhook key.</returns>
        Task<WebhookKey?> GetKeyAsync(string keyId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Invalidates a cached key.
        /// </summary>
        /// <param name="keyId">The key identifier to invalidate.</param>
        void InvalidateKey(string keyId);

        /// <summary>
        ///     Clears all cached keys.
        /// </summary>
        void Clear();
    }
}