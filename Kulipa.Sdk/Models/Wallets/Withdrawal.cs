using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Wallets
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
        ///     Current status of the withdrawal transaction.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WithdrawalStatus Status { get; set; }

        /// <summary>
        ///     Token identifier for the withdrawal. Represents the cryptocurrency or token being withdrawn. TODO
        /// </summary>
        [JsonPropertyName("tokenId")]
        public string TokenId { get; set; }

        /// <summary>
        ///     Smart contract address of the token being withdrawn. TODO
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public string ContractAddress { get; set; }

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

    /// <summary>
    ///     Possible statuses of a withdrawal transaction.
    /// </summary>
    public enum WithdrawalStatus
    {
        /// <summary>
        ///     Withdrawal is in draft state.
        /// </summary>
        Draft,

        /// <summary>
        ///     Withdrawal is pending confirmation.
        /// </summary>
        Pending,

        /// <summary>
        ///     Withdrawal has been confirmed and processed.
        /// </summary>
        Confirmed,

        /// <summary>
        ///     Withdrawal has been rejected.
        /// </summary>
        Rejected,

        /// <summary>
        ///     Withdrawal has failed to process.
        /// </summary>
        Failed
    }
}