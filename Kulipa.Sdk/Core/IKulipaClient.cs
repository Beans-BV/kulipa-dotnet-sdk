using Kulipa.Sdk.Resources;

namespace Kulipa.Sdk.Core
{
    /// <summary>
    ///     Main interface for interacting with the Kulipa API.
    /// </summary>
    public interface IKulipaClient : IDisposable
    {
        /// <summary>
        ///     Gets the Users resource for managing user operations.
        /// </summary>
        IUsersResource Users { get; }

        /// <summary>
        ///     Gets the Cards resource for managing card operations.
        /// </summary>
        ICardsResource Cards { get; }

        /// <summary>
        ///     Gets the Transactions resource for managing transactions.
        /// </summary>
        ITransactionsResource Transactions { get; }

        /// <summary>
        ///     Gets the webhooks resource for webhook operations.
        /// </summary>
        IWebhooksResource Webhooks { get; }

        /// <summary>
        ///     Gets the Wallet resource for managing wallet addresses and balances.
        /// </summary>
        IWalletsResource Wallets { get; }

        /// <summary>
        ///     Tests the API connection with the current configuration.
        /// </summary>
        Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);
    }
}