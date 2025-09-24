using Kulipa.Sdk.Configuration;
using Microsoft.Extensions.Options;

namespace Kulipa.Sdk.Services.Authentication
{
    /// <summary>
    ///     HTTP message handler that adds API key authentication to requests.
    /// </summary>
    public class ApiKeyAuthenticationHandler : DelegatingHandler
    {
        private const string ApiKeyHeader = "x-api-key";
        private readonly KulipaSdkOptions _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The SDK configuration options containing the API key.</param>
        public ApiKeyAuthenticationHandler(IOptions<KulipaSdkOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        ///     Sends an HTTP request with API key authentication header added.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The HTTP response message.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_options.ApiKey))
            {
                request.Headers.Add(ApiKeyHeader, _options.ApiKey);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}