using System.Collections.Concurrent;
using System.Net.Http.Json;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Models.Webhooks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kulipa.Sdk.Webhooks
{
    /// <summary>
    ///     In-memory implementation of webhook public key cache.
    /// </summary>
    public class MemoryPublicKeyCache : IPublicKeyCache
    {
        private readonly ConcurrentDictionary<string, CachedKey> _cache;
        private readonly TimeSpan _cacheExpiration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MemoryPublicKeyCache> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryPublicKeyCache" /> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for API calls.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The SDK options.</param>
        public MemoryPublicKeyCache(
            HttpClient httpClient,
            ILogger<MemoryPublicKeyCache> logger,
            IOptions<KulipaSdkOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = new ConcurrentDictionary<string, CachedKey>();
            _cacheExpiration = options?.Value?.WebhookKeyCacheExpiration ?? TimeSpan.FromHours(1);
        }

        /// <inheritdoc />
        public async Task<WebhookKey?> GetKeyAsync(string keyId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(keyId))
            {
                _logger.LogWarning("Attempted to fetch key with null or empty ID");
                return null;
            }

            // Check cache first
            if (_cache.TryGetValue(keyId, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
            {
                _logger.LogDebug("Returning cached public key for ID: {KeyId}", keyId);
                return cached.Key;
            }

            // Fetch from API
            try
            {
                _logger.LogInformation("Fetching public key from API for ID: {KeyId}", keyId);

                var response = await _httpClient.GetAsync(
                    $"webhooks/keys/{keyId}",
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch public key. Status: {StatusCode}, KeyId: {KeyId}",
                        response.StatusCode, keyId);
                    return null;
                }

                var key = await response.Content.ReadFromJsonAsync<WebhookKey>(cancellationToken);

                if (key == null)
                {
                    _logger.LogWarning("Received null key from API for ID: {KeyId}", keyId);
                    return null;
                }

                // Cache the key
                var cachedEntry = new CachedKey
                {
                    Key = key,
                    ExpiresAt = DateTime.UtcNow.Add(_cacheExpiration)
                };

                _cache.AddOrUpdate(keyId, cachedEntry, (_, _) => cachedEntry);
                _logger.LogDebug("Cached public key for ID: {KeyId}, expires at: {ExpiresAt}",
                    keyId, cachedEntry.ExpiresAt);

                return key;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while fetching public key for ID: {KeyId}", keyId);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout while fetching public key for ID: {KeyId}", keyId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching public key for ID: {KeyId}", keyId);
                return null;
            }
        }

        /// <inheritdoc />
        public void InvalidateKey(string keyId)
        {
            if (string.IsNullOrWhiteSpace(keyId))
            {
                return;
            }

            if (_cache.TryRemove(keyId, out _))
            {
                _logger.LogDebug("Invalidated cached key for ID: {KeyId}", keyId);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            var count = _cache.Count;
            _cache.Clear();
            _logger.LogInformation("Cleared {Count} cached keys", count);
        }

        private class CachedKey
        {
            public WebhookKey Key { get; set; } = null!;
            public DateTime ExpiresAt { get; set; }
        }
    }
}