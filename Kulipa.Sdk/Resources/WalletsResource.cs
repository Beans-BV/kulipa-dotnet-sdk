using System.Collections.Specialized;
using Kulipa.Sdk.Models.Common;
using Kulipa.Sdk.Models.Wallets;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Implementation of wallet operations.
    /// </summary>
    public class WalletsResource : BaseResource, IWalletsResource
    {
        /// <inheritdoc />
        public WalletsResource(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <inheritdoc />
        public async Task<Wallet> CreateAsync(
            CreateWalletRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            return await PostAsync<CreateWalletRequest, Wallet>(
                "wallets",
                request,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Wallet> GetAsync(
            string walletId,
            CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync<Wallet>(
                $"wallets/{walletId}",
                walletId,
                nameof(walletId),
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Wallet>> ListAsync(
            string? userId = null,
            string? address = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            var queryParameters = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(userId))
            {
                queryParameters["userId"] = userId;
            }

            if (!string.IsNullOrEmpty(address))
            {
                queryParameters["address"] = address;
            }

            return await ListAsync<Wallet>(
                "wallets",
                queryParameters,
                pagedRequest,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Wallet> VerifyAsync(
            string walletId,
            object typedData,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(walletId, nameof(walletId));
            ArgumentNullException.ThrowIfNull(typedData);

            return await PutAsync<object, Wallet>(
                $"wallets/{walletId}/verify",
                typedData,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Withdrawal> WithdrawFundAsync(
            string walletId,
            WithdrawFundRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(walletId, nameof(walletId));
            ArgumentNullException.ThrowIfNull(request);

            if (request.Amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Amount),
                    "Amount must be greater than zero");
            }

            return await PostAsync<object, Withdrawal>(
                $"wallets/{walletId}/withdrawals",
                request,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Withdrawal>> ListWithdrawalsAsync(
            string walletId,
            DateTime? createdSince = null,
            IEnumerable<WithdrawalStatus>? statuses = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(walletId, nameof(walletId));

            Action<NameValueCollection> queryBuilder = query =>
            {
                // Add date filtering
                if (createdSince.HasValue)
                {
                    query["createdSince"] = createdSince.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }

                if (statuses == null)
                {
                    return;
                }

                // Add status filtering
                var statusList = statuses.ToList();
                if (statusList.Count <= 0)
                {
                    return;
                }

                foreach (var status in statusList)
                {
                    query.Add("statuses", status.ToString().ToLowerInvariant());
                }
            };

            return await ListAsync<Withdrawal>(
                $"wallets/{walletId}/withdrawals",
                queryBuilder,
                pagedRequest,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedResponse<TopUp>> ListTopUpsAsync(
            string walletId,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(walletId, nameof(walletId));

            return await ListAsync<TopUp>(
                $"wallets/{walletId}/top-ups",
                new Dictionary<string, string?>(),
                pagedRequest, cancellationToken);
        }
    }
}