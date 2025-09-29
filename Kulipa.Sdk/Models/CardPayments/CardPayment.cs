using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.CardPayments
{
    /// <summary>
    ///     Represents a card payment transaction in the Kulipa system.
    /// </summary>
    public class CardPayment
    {
        /// <summary>
        ///     Unique identifier for the card payment.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        ///     The type of payment transaction (payment or refund).
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardPaymentType Type { get; set; }

        /// <summary>
        ///     The status of the payment during its lifecycle.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardPaymentStatus Status { get; set; }

        /// <summary>
        ///     In case the payment is rejected, this would supply a reason for the rejection.
        /// </summary>
        [JsonPropertyName("declineReason")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardPaymentDeclineReason DeclineReason { get; set; }

        /// <summary>
        ///     Unique identifier for the transaction's card.
        /// </summary>
        [JsonPropertyName("cardId")]
        public string CardId { get; set; } = null!;

        /// <summary>
        ///     Unique identifier for the transaction's card holder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = null!;

        /// <summary>
        ///     The payment amount, which includes the already-settled amount as well as still-held funds. If the payment is in
        ///     status complete, then the whole amount is settled. This amount is presented in its minimal currency denomination.
        ///     108.62 USD would be represented as 10862.
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        ///     The original authorized amount. This amount is presented in its minimal currency denomination. 108.62 USD would be
        ///     represented as 10862
        /// </summary>
        [JsonPropertyName("originalAmount")]
        public decimal OriginalAmount { get; set; }

        /// <summary>
        ///     The original authorized amount, expressed in merchantCurrency and in typical units of the currency (i.e. fractions
        ///     possible). This will differ from originalAmount if the merchant processes payments in an alternate currency. This
        ///     is not guaranteed to, but will almost always be filled.
        /// </summary>
        [JsonPropertyName("originalMerchantAmount")]
        public string? OriginalMerchantAmount { get; set; }

        /// <summary>
        ///     The date when the authorization expires if not cleared.
        /// </summary>
        [JsonPropertyName("expirationAt")]
        public DateTime? ExpirationAt { get; set; }

        /// <summary>
        ///     Three-letter ISO currency code describing the currency of the payment amount.
        /// </summary>
        [StringLength(3, MinimumLength = 3)]
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        /// <summary>
        ///     The amount that the merchant will obtain, expressed in merchantCurrency and in typical units of the currency (i.e.
        ///     fractions possible). This will differ from amount if the merchant processes payments in an alternate currency. This
        ///     is not guaranteed to, but will almost always be filled.
        /// </summary>
        [JsonPropertyName("merchantAmount")]
        public string? MerchantAmount { get; set; }

        /// <summary>
        ///     The currency, denoted by the ISO 4217 code, in which the merchant will be paid. (i.e. the "original" currency of
        ///     the transaction.
        ///     Length: 3 characters.
        /// </summary>
        [StringLength(3, MinimumLength = 3)]
        [JsonPropertyName("merchantCurrency")]
        public string? MerchantCurrency { get; set; }

        /// <summary>
        ///     Information about the merchant involved in the payment.
        /// </summary>
        [JsonPropertyName("merchant")]
        public Merchant? Merchant { get; set; }

        /// <summary>
        ///     The date the cardholder made the payment.
        /// </summary>
        [JsonPropertyName("paymentDateTime")]
        public DateTime? PaymentDateTime { get; set; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}