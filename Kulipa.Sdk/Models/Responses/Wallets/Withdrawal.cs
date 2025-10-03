using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Wallets
{
    /// <summary>
    ///     Represents a Kulipa wallet withdrawal transaction.
    /// </summary>
    public sealed record Withdrawal
    {
        /// <summary>
        ///     Withdrawal identifier. Begins with 'wdr-' followed by a v4 UUID.
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
        ///     Current status of the withdrawal transaction.
        /// </summary>
        [JsonPropertyName("status")]
        public WithdrawalStatus Status { get; init; }

        /// <summary>
        ///     Token identifier for the withdrawal. Represents the cryptocurrency or token being withdrawn.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("tokenId")]
        public required string TokenId { get; init; }

        /// <summary>
        ///     Smart contract address of the token being withdrawn.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public required string ContractAddress { get; init; }

        /// <summary>
        ///     Blockchain transaction hash for this withdrawal.
        /// </summary>
        [JsonPropertyName("hash")]
        public string? Hash { get; init; }

        /// <summary>
        ///     Amount to withdraw in token decimal precision (e.g., 1000000 for 1 USDC).
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; init; }

        /// <summary>
        ///     Last update date and time of the withdrawal.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; init; }

        /// <summary>
        ///     Date and time when the withdrawal was confirmed (if applicable).
        /// </summary>
        [JsonPropertyName("confirmedAt")]
        public DateTime? ConfirmedAt { get; init; }
    }
}