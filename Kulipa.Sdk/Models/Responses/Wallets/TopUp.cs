using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Wallets
{
    /// <summary>
    ///     Represents a Kulipa wallet top-up.
    /// </summary>
    public sealed record TopUp
    {
        /// <summary>
        ///     A Blockchain Transaction identifier. Begins with 'btn-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; init; } = null!;

        /// <summary>
        ///     The UUID of the user's wallet.
        /// </summary>
        [JsonPropertyName("walletId")]
        public string WalletId { get; init; } = null!;

        /// <summary>
        ///     Status of the top-up transaction.
        /// </summary>
        [JsonPropertyName("status")]
        public TopUpStatus Status { get; init; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public string ContractAddress { get; init; } = null!;

        /// <summary>
        ///     A blockchain on which the wallet is deployed.
        /// </summary>
        [JsonPropertyName("blockchain")]
        public BlockchainNetwork Blockchain { get; init; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("transactionHash")]
        public string TransactionHash { get; init; } = null!;

        /// <summary>
        ///     The block number where the transaction was included.
        /// </summary>
        [JsonPropertyName("includedInBlockNumber")]
        public bool? IncludedInBlockNumber { get; init; }

        /// <summary>
        ///     Amount to withdraw
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; init; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; init; }
    }
}