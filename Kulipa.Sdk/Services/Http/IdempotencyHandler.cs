using System.Security.Cryptography;
using Kulipa.Sdk.Configuration;
using Microsoft.Extensions.Options;

namespace Kulipa.Sdk.Services.Http
{
    /// <summary>
    ///     Handles idempotency for POST and PUT requests.
    /// </summary>
    public class IdempotencyHandler : DelegatingHandler
    {
        private const string IdempotencyKeyHeader = "x-idempotency-key";
        private const int MaxIdempotencyKeyLength = 64;
        private readonly KulipaSdkOptions _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdempotencyHandler" /> class.
        /// </summary>
        /// <param name="options">The SDK configuration options containing idempotency settings.</param>
        public IdempotencyHandler(IOptions<KulipaSdkOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        ///     Sends an HTTP request with idempotency key header added for POST and PUT requests.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>The HTTP response message.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!_options.EnableIdempotency ||
                (request.Method != HttpMethod.Post && request.Method != HttpMethod.Put))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            string? idempotencyKey = null;

            // Check if header already exists
            if (request.Headers.TryGetValues(IdempotencyKeyHeader, out var existingValues))
            {
                idempotencyKey = existingValues.FirstOrDefault();
            }
            else
            {
                // Try to get from request options
                if (request.Options.TryGetValue(
                        new HttpRequestOptionsKey<string>("IdempotencyKey"), out var key))
                {
                    idempotencyKey = key;
                }
                else if (_options.AutoGenerateIdempotencyKey)
                {
                    idempotencyKey = GenerateIdempotencyKey(request);
                }
            }

            // Validate the key
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            if (idempotencyKey.Length > MaxIdempotencyKeyLength)
            {
                // Remove existing header if present
                request.Headers.Remove(IdempotencyKeyHeader);
                throw new ArgumentException(
                    $"Idempotency key exceeds maximum length of {MaxIdempotencyKeyLength} characters. " +
                    $"Provided key length: {idempotencyKey.Length}. " +
                    $"Consider using a hash of your key or enable auto-truncation.",
                    nameof(idempotencyKey));
            }

            if (!request.Headers.Contains(IdempotencyKeyHeader))
            {
                // Only add if not already present and valid
                request.Headers.Add(IdempotencyKeyHeader, idempotencyKey);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        ///     Generates a unique idempotency key based on the HTTP request method, URL, and timestamp.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <returns>A unique idempotency key string.</returns>
        private static string GenerateIdempotencyKey(HttpRequestMessage request)
        {
            // Generate a unique key based on method, URL, and timestamp
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var guid = Guid.NewGuid().ToString("N");
            var methodAndUrl = $"{request.Method}_{request.RequestUri?.PathAndQuery}";

            // Create combined string and hash it to ensure max 64 chars
            var combined = $"{methodAndUrl}_{guid}_{timestamp}";
            var hashBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(combined));
            return Convert.ToHexString(hashBytes);
        }
    }
}