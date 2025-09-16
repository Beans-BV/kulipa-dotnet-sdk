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

        public ApiKeyAuthenticationHandler(IOptions<KulipaSdkOptions> options)
        {
            _options = options.Value;
        }

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