using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Common;

namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Represents a Kulipa wallet top-up.
    /// </summary>
    public class TopUp
    {
        /// <summary>
        ///     A Blockchain Transaction identifier. Begins with 'btn-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        /// <summary>
        ///     The UUID of the user's wallet.
        /// </summary>
        [JsonPropertyName("walletId")]
        public required string WalletId { get; set; }

        /// <summary>
        ///     Status of the top-up transaction.
        /// </summary>
        [JsonPropertyName("status")]
        public TopUpStatus Status { get; set; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public string ContractAddress { get; set; }

        /// <summary>
        ///     A blockchain on which the wallet is deployed.
        /// </summary>
        [JsonPropertyName("blockchain")]
        public BlockchainNetwork Blockchain { get; set; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("transactionHash")]
        public string TransactionHash { get; set; }

        /// <summary>
        ///     The block number where the transaction was included.
        /// </summary>
        [JsonPropertyName("includedInBlockNumber")]
        public bool? IncludedInBlockNumber { get; set; }

        /// <summary>
        ///     Amount to withdraw
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}