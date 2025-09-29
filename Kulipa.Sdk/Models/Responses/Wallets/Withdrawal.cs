using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Wallets
{
    /// <summary>
    ///     Represents a Kulipa wallet withdrawal transaction.
    /// </summary>
    public class Withdrawal
    {
        /// <summary>
        ///     Withdrawal identifier. Begins with 'wdr-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = null!;

        /// <summary>
        ///     The UUID of the user's wallet.
        /// </summary>
        [JsonPropertyName("walletId")]
        public string WalletId { get; set; } = null!;

        /// <summary>
        ///     Current status of the withdrawal transaction.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WithdrawalStatus Status { get; set; }

        /// <summary>
        ///     Token identifier for the withdrawal. Represents the cryptocurrency or token being withdrawn.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("tokenId")]
        public string TokenId { get; set; } = null!;

        /// <summary>
        ///     Smart contract address of the token being withdrawn.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public string ContractAddress { get; set; } = null!;

        /// <summary>
        ///     Blockchain transaction hash for this withdrawal.
        /// </summary>
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        /// <summary>
        ///     Amount to withdraw in token decimal precision (e.g., 1000000 for 1 USDC).
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Last update date and time of the withdrawal.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        ///     Date and time when the withdrawal was confirmed (if applicable).
        /// </summary>
        [JsonPropertyName("confirmedAt")]
        public DateTime? ConfirmedAt { get; set; }
    }
}