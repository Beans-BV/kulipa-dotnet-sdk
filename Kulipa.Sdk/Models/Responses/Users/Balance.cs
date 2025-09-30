using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Users
{
    /// <summary>
    ///     Represents a user balance.
    /// </summary>
    public sealed record Balance
    {
        /// <summary>
        ///     Onchain balance minus card pending holds, pending transfer and erc20 allowances.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("accountUsableBalance")]
        public decimal AccountUsableBalance { get; init; }

        /// <summary>
        ///     User pending balance in currency (card pending holds, pending transfer and erc20 allowances).
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("reservedBalance")]
        public decimal ReservedBalance { get; init; }

        /// <summary>
        ///     Balance that can be spent in currency. It is the minimum between the account net balance and the card limits
        ///     remaining amount.
        /// </summary>
        [JsonPropertyName("cardUsableBalance")]
        public decimal CardUsableBalance { get; init; }

        /// <summary>
        ///     Balance that can be spent in currency during the day. It is the minimum between the account net balance and the
        ///     daily card limits remaining amount.
        /// </summary>
        [JsonPropertyName("cardDailyUsableBalance")]
        public decimal CardDailyUsableBalance { get; init; }

        /// <summary>
        ///     Balance that can be spent in currency during the month. It is the minimum between the account net balance and the
        ///     monthly card limits remaining amount.
        /// </summary>
        [JsonPropertyName("cardMonthlyUsableBalance")]
        public decimal CardMonthlyUsableBalance { get; init; }

        /// <summary>
        ///     Onchain balance.
        /// </summary>
        [JsonPropertyName("bookedBalance")]
        public decimal BookedBalance { get; init; }

        /// <summary>
        ///     Card pending holds.
        /// </summary>
        [JsonPropertyName("pendingCardTransactions")]
        public decimal PendingCardTransactions { get; init; }

        /// <summary>
        ///     Pending transfer and erc20 allowances.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("pendingWalletTransactions")]
        public decimal PendingWalletTransactions { get; init; }

        /// <summary>
        ///     User balance currency code with decimal precision.
        ///     Currently, there is only one supported currency, "USD6D" which means USD with 6 decimal numbers precision (int).
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; init; } = null!;
    }
}