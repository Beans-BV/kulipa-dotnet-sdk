using Kulipa.Sdk.Models.Enums;
using Kulipa.Sdk.Models.Requests.Common;
using Kulipa.Sdk.Models.Requests.Wallets;
using Kulipa.Sdk.Models.Responses.Common;
using Kulipa.Sdk.Models.Responses.Wallets;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Interface for wallets.
    /// </summary>
    public interface IWalletsResource
    {
        /// <summary>
        ///     A POST request sent to create a wallet.
        /// </summary>
        /// <param name="request">Wallet object that needs to be created/issued for an owner.</param>
        /// <param name="idempotencyKey">Optional idempotency key for the request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created wallet.</returns>
        /// <remarks>
        ///     Whenever a non-custodial wallet address is provided to this endpoint, the client will have to verify the
        ///     wallet ownership with the /wallets/:id/verify endpoint.
        /// </remarks>
        /// <remarks>
        ///     When creating a wallet, you must provide either the address property (for debit accounts) or the
        ///     withdrawalAddress property (for prepaid accounts), depending on the type of wallet you are creating.
        /// </remarks>
        Task<Wallet> CreateAsync(
            CreateWalletRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Fetch a wallet entity.
        /// </summary>
        /// <param name="walletId">The UUID of the wallet.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The wallet details.</returns>
        Task<Wallet> GetAsync(
            string walletId,
            CancellationToken cancellationToken = default);


        /// <summary>
        ///     Lists wallets with optional filtering.
        /// </summary>
        /// <param name="userId">Optional user ID to filter by.</param>
        /// <param name="address">The wallet address.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged list of wallets.</returns>
        Task<PagedResponse<Wallet>> ListAsync(
            string? userId = null,
            string? address = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        ///     A PUT request sent to verify a non-custodial wallet.
        ///     <param name="walletId">The UUID of the wallet.</param>
        ///     <param name="typedData">TODO: Update this when Kulipa updates their docs after adding support for Stellar.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Wallet> VerifyAsync(
            string walletId,
            object typedData,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     A POST request to withdraw funds to a registered non-custodial wallet.
        ///     <param name="walletId">The UUID of the wallet.</param>
        ///     <param name="request">Withdrawal reject to be created.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Withdrawal> WithdrawFundAsync(
            string walletId,
            WithdrawFundRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     A GET request to list withdrawals from a registered non-custodial wallet.
        ///     Prepaid program ONLY.
        /// </summary>
        /// <param name="walletId">[Required] The UUID of the wallet.</param>
        /// <param name="createdSince">Filter withdrawals created since this date and time.</param>
        /// <param name="statuses">Filter withdrawals by status. Multiple statuses can be specified.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged list of withdrawals.</returns>
        Task<PagedResponse<Withdrawal>> ListWithdrawalsAsync(
            string walletId,
            DateTime? createdSince = null,
            IEnumerable<WithdrawalStatus>? statuses = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     A GET request to list blockchain deposit transactions (top-ups) to a registered wallet.
        /// </summary>
        /// <param name="walletId">[Required] The UUID of the wallet.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged list of top-ups.</returns>
        Task<PagedResponse<TopUp>> ListTopUpsAsync(
            string walletId,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default);
    }
}