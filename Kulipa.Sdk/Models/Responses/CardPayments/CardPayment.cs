using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.CardPayments
{
    /// <summary>
    ///     Represents a card payment transaction in the Kulipa system.
    /// </summary>
    public sealed record CardPayment
    {
        /// <summary>
        ///     Unique identifier for the card payment.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        /// <summary>
        ///     The type of payment transaction (payment or refund).
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardPaymentType Type { get; init; }

        /// <summary>
        ///     The status of the payment during its lifecycle.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardPaymentStatus Status { get; init; }

        /// <summary>
        ///     In case the payment is rejected, this would supply a reason for the rejection.
        /// </summary>
        [JsonPropertyName("declineReason")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardPaymentDeclineReason DeclineReason { get; init; }

        /// <summary>
        ///     Unique identifier for the transaction's card.
        /// </summary>
        [JsonPropertyName("cardId")]
        public required string CardId { get; init; }

        /// <summary>
        ///     Unique identifier for the transaction's card holder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; init; }

        /// <summary>
        ///     The payment amount, which includes the already-settled amount as well as still-held funds. If the payment is in
        ///     status complete, then the whole amount is settled. This amount is presented in its minimal currency denomination.
        ///     108.62 USD would be represented as 10862.
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        /// <summary>
        ///     The original authorized amount. This amount is presented in its minimal currency denomination. 108.62 USD would be
        ///     represented as 10862
        /// </summary>
        [JsonPropertyName("originalAmount")]
        public decimal OriginalAmount { get; init; }

        /// <summary>
        ///     The original authorized amount, expressed in merchantCurrency and in typical units of the currency (i.e. fractions
        ///     possible). This will differ from originalAmount if the merchant processes payments in an alternate currency. This
        ///     is not guaranteed to, but will almost always be filled.
        /// </summary>
        [JsonPropertyName("originalMerchantAmount")]
        public string? OriginalMerchantAmount { get; init; }

        /// <summary>
        ///     The date when the authorization expires if not cleared.
        /// </summary>
        [JsonPropertyName("expirationAt")]
        public DateTime? ExpirationAt { get; init; }

        /// <summary>
        ///     Three-letter ISO currency code describing the currency of the payment amount.
        /// </summary>
        [JsonPropertyName("currency")]
        public string? Currency { get; init; }

        /// <summary>
        ///     The amount that the merchant will obtain, expressed in merchantCurrency and in typical units of the currency (i.e.
        ///     fractions possible). This will differ from amount if the merchant processes payments in an alternate currency. This
        ///     is not guaranteed to, but will almost always be filled.
        /// </summary>
        [JsonPropertyName("merchantAmount")]
        public string? MerchantAmount { get; init; }

        /// <summary>
        ///     The currency, denoted by the ISO 4217 code, in which the merchant will be paid. (i.e. the "original" currency of
        ///     the transaction.
        ///     Length: 3 characters.
        /// </summary>
        [JsonPropertyName("merchantCurrency")]
        public string? MerchantCurrency { get; init; }

        /// <summary>
        ///     Information about the merchant involved in the payment.
        /// </summary>
        [JsonPropertyName("merchant")]
        public Merchant? Merchant { get; init; }

        /// <summary>
        ///     The date the cardholder made the payment.
        /// </summary>
        [JsonPropertyName("paymentDateTime")]
        public DateTime? PaymentDateTime { get; init; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; init; }
    }
}