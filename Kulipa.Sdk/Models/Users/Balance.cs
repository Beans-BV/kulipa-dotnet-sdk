using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Users
{
    /// <summary>
    ///     Represents a user balance.
    /// </summary>
    public class Balance
    {
        /// <summary>
        ///     Onchain balance minus card pending holds, pending transfer and erc20 allowances.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("accountUsableBalance")]
        public required decimal AccountUsableBalance { get; set; }

        /// <summary>
        ///     User pending balance in currency (card pending holds, pending transfer and erc20 allowances).
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("reservedBalance")]
        public required decimal ReservedBalance { get; set; }

        /// <summary>
        ///     Balance that can be spent in currency. It is the minimum between the account net balance and the card limits
        ///     remaining amount.
        /// </summary>
        [JsonPropertyName("cardUsableBalance")]
        public required decimal CardUsableBalance { get; set; }

        /// <summary>
        ///     Balance that can be spent in currency during the day. It is the minimum between the account net balance and the
        ///     daily card limits remaining amount.
        /// </summary>
        [JsonPropertyName("cardDailyUsableBalance")]
        public required decimal CardDailyUsableBalance { get; set; }

        /// <summary>
        ///     Balance that can be spent in currency during the month. It is the minimum between the account net balance and the
        ///     monthly card limits remaining amount.
        /// </summary>
        [JsonPropertyName("cardMonthlyUsableBalance")]
        public required decimal CardMonthlyUsableBalance { get; set; }

        /// <summary>
        ///     Onchain balance.
        /// </summary>
        [JsonPropertyName("bookedBalance")]
        public decimal? BookedBalance { get; set; }

        /// <summary>
        ///     Card pending holds.
        /// </summary>
        [JsonPropertyName("pendingCardTransactions")]
        public required decimal PendingCardTransactions { get; set; }

        /// <summary>
        ///     Pending transfer and erc20 allowances.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("pendingWalletTransactions")]
        public required decimal PendingWalletTransactions { get; set; }

        /// <summary>
        ///     User balance currency code with decimal precision.
        ///     Currently, there is only one supported currency, "USD6D" which means USD with 6 decimal numbers precision (int).
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("currency")]
        public required string Currency { get; set; }
    }
}