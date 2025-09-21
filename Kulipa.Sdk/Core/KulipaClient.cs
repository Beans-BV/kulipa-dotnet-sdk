using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Resources;
using Microsoft.Extensions.Options;

namespace Kulipa.Sdk.Core
{
    /// <summary>
    ///     Main client for interacting with the Kulipa API.
    /// </summary>
    public class KulipaClient : IKulipaClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KulipaClient> _logger;
        private readonly KulipaSdkOptions _options;
        private bool _disposed;


        public KulipaClient(
            HttpClient httpClient,
            IOptions<KulipaSdkOptions> options,
            ILogger<KulipaClient> logger,
            IUsersResource usersResource,
            IWalletsResource walletsResource,
            IKycsResource kycsResource,
            ICardsResource cardsResource,
            ICardPaymentsResource cardPaymentsResource,
            IWebhooksResource webhooksResource
        )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Users = usersResource ?? throw new ArgumentNullException(nameof(usersResource));
            Wallets = walletsResource ?? throw new ArgumentNullException(nameof(walletsResource));
            Kycs = kycsResource ?? throw new ArgumentNullException(nameof(kycsResource));
            Cards = cardsResource ?? throw new ArgumentNullException(nameof(cardsResource));
            CardPayments = cardPaymentsResource ?? throw new ArgumentNullException(nameof(cardPaymentsResource));
            Webhooks = webhooksResource ?? throw new ArgumentNullException(nameof(webhooksResource)); // Add this

            _logger.LogInformation("Kulipa SDK initialized for {Environment} environment", _options.Environment);
        }

        public IKycsResource Kycs { get; }

        public IUsersResource Users { get; }
        public IWalletsResource Wallets { get; }
        public ICardsResource Cards { get; }
        public ICardPaymentsResource CardPayments { get; }
        public IWebhooksResource Webhooks { get; }

        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Testing API connection");

                // Try to list cards with limit 1 as a connection test
                var response = await _httpClient.GetAsync("/cards?limit=1", cancellationToken);

                var success = response.IsSuccessStatusCode;

                _logger.LogInformation("API connection test {Result}", success ? "succeeded" : "failed");

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API connection test failed with exception");
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // HttpClient is managed by DI container, so we don't dispose it here
                    _logger.LogDebug("Kulipa SDK disposed");
                }

                _disposed = true;
            }
        }
    }
}