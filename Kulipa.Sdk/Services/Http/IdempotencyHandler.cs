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
        private readonly KulipaSdkOptions _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdempotencyHandler"/> class.
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
        /// <returns>The HTTP response message.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Add idempotency key for POST and PUT requests
            if (_options.EnableIdempotency &&
                (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put))
            {
                // Check if idempotency key is already set (from request context)
                if (!request.Headers.Contains(IdempotencyKeyHeader))
                {
                    // Check if there's a key in the request properties
                    if (request.Options.TryGetValue(new HttpRequestOptionsKey<string>("IdempotencyKey"), out var key))
                    {
                        request.Headers.Add(IdempotencyKeyHeader, key);
                    }
                    else if (_options.AutoGenerateIdempotencyKey)
                    {
                        // Auto-generate a key based on request content
                        var generatedKey = GenerateIdempotencyKey(request);
                        request.Headers.Add(IdempotencyKeyHeader, generatedKey);
                    }
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        ///     Generates a unique idempotency key based on the HTTP request method, URL, and timestamp.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <returns>A unique idempotency key string.</returns>
        private string GenerateIdempotencyKey(HttpRequestMessage request)
        {
            // Generate a unique key based on method, URL, and timestamp
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var guid = Guid.NewGuid().ToString("N");
            return $"{request.Method}_{request.RequestUri?.PathAndQuery}_{guid}_{timestamp}";
        }
    }
}