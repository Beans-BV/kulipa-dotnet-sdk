using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Wallets
{
    /// <summary>
    ///     Represents a wallet balance.
    /// </summary>
    public sealed record WalletBalance
    {
        /// <summary>
        ///     Onchain balance minus card pending holds, pending transfer and erc20 allowances.
        /// </summary>
        [JsonPropertyName("accountUsableBalance")]
        public required long AccountUsableBalance { get; init; }

        /// <summary>
        ///     Wallet pending balance in currency (card pending holds, pending transfer and erc20 allowances).
        /// </summary>
        [JsonPropertyName("reservedBalance")]
        public required long ReservedBalance { get; init; }

        /// <summary>
        ///     Onchain balance.
        /// </summary>
        [JsonPropertyName("bookedBalance")]
        public required long BookedBalance { get; init; }

        /// <summary>
        ///     Card pending holds.
        /// </summary>
        [JsonPropertyName("pendingCardTransactions")]
        public required long PendingCardTransactions { get; init; }

        /// <summary>
        ///     Pending transfer and erc20 allowances.
        /// </summary>
        [JsonPropertyName("pendingWalletTransactions")]
        public required long PendingWalletTransactions { get; init; }

        /// <summary>
        ///     Wallet balance currency code with decimal precision.
        ///     Currently, there is only one supported currency, "USD6D" which means USD with 6 decimal numbers precision.
        /// </summary>
        [JsonPropertyName("currency")]
        public required string Currency { get; init; }
    }
}
