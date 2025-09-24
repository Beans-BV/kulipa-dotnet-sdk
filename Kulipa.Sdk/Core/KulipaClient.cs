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


        /// <summary>
        ///     Initializes a new instance of the <see cref="KulipaClient" /> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making API requests.</param>
        /// <param name="options">The SDK configuration options.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="usersResource">The users resource implementation.</param>
        /// <param name="walletsResource">The wallets resource implementation.</param>
        /// <param name="kycsResource">The KYCs resource implementation.</param>
        /// <param name="cardsResource">The cards resource implementation.</param>
        /// <param name="cardPaymentsResource">The card payments resource implementation.</param>
        /// <param name="webhooksResource">The webhooks resource implementation.</param>
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

        /// <inheritdoc />
        public IKycsResource Kycs { get; }

        /// <inheritdoc />
        public IUsersResource Users { get; }

        /// <inheritdoc />
        public IWalletsResource Wallets { get; }

        /// <inheritdoc />
        public ICardsResource Cards { get; }

        /// <inheritdoc />
        public ICardPaymentsResource CardPayments { get; }

        /// <inheritdoc />
        public IWebhooksResource Webhooks { get; }

        /// <inheritdoc />
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

        /// <inheritdoc />
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