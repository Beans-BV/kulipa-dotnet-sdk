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
        public required string Id { get; init; }

        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; init; }

        /// <summary>
        ///     The UUID of the user's wallet.
        /// </summary>
        [JsonPropertyName("walletId")]
        public required string WalletId { get; init; }

        /// <summary>
        ///     Status of the top-up transaction.
        /// </summary>
        [JsonPropertyName("status")]
        public TopUpStatus Status { get; init; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public required string ContractAddress { get; init; }

        /// <summary>
        ///     A blockchain on which the wallet is deployed.
        /// </summary>
        [JsonPropertyName("blockchain")]
        public BlockchainNetwork Blockchain { get; init; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("transactionHash")]
        public required string TransactionHash { get; init; }

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