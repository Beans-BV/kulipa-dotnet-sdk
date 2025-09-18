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
        public required string Id { get; set; }

        /// <summary>
        ///     The type of payment transaction (payment or refund).
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required CardPaymentType Type { get; set; }

        /// <summary>
        ///     The status of the payment during its lifecycle.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required CardPaymentStatus Status { get; set; }

        /// <summary>
        ///     In case the payment is rejected, this would supply a reason for the rejection.
        /// </summary>
        [JsonPropertyName("declineReason")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required CardPaymentDeclineReason DeclineReason { get; set; }

        /// <summary>
        ///     Unique identifier for the transaction's card.
        /// </summary>
        [JsonPropertyName("cardId")]
        public required string CardId { get; set; }

        /// <summary>
        ///     Unique identifier for the transaction's card holder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        /// <summary>
        ///     The payment amount, which includes the already-settled amount as well as still-held funds. If the payment is in
        ///     status complete, then the whole amount is settled. This amount is presented in its minimal currency denomination.
        ///     108.62 USD would be represented as 10862.
        /// </summary>
        [JsonPropertyName("amount")]
        public required decimal Amount { get; set; }

        /// <summary>
        ///     The original authorized amount. This amount is presented in its minimal currency denomination. 108.62 USD would be
        ///     represented as 10862
        /// </summary>
        [JsonPropertyName("originalAmount")]
        public required decimal OriginalAmount { get; set; }

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

    /// <summary>
    ///     Possible statuses of a card payment.
    /// </summary>
    public enum CardPaymentStatus
    {
        /// <summary>
        ///     Payment has been confirmed and is being processed.
        /// </summary>
        Confirmed,

        /// <summary>
        ///     Payment has been completed successfully.
        /// </summary>
        Complete,

        /// <summary>
        ///     Payment has been rejected.
        /// </summary>
        Rejected,

        /// <summary>
        ///     Payment has failed due to an error.
        /// </summary>
        Failed
    }

    /// <summary>
    ///     Types of card payment transactions.
    /// </summary>
    public enum CardPaymentType
    {
        /// <summary>
        ///     A regular payment transaction.
        /// </summary>
        Payment,

        /// <summary>
        ///     A refund transaction.
        /// </summary>
        Refund
    }

    /// <summary>
    ///     Reasons why a card payment might be declined.
    /// </summary>
    public enum CardPaymentDeclineReason
    {
        /// <summary>
        ///     Insufficient funds in the account.
        /// </summary>
        InsufficientFund,

        /// <summary>
        ///     Payment failed fraud or security controls.
        /// </summary>
        FailedControl,

        /// <summary>
        ///     Card or account is not active.
        /// </summary>
        NotActive,

        /// <summary>
        ///     Eager transfer timed out.
        /// </summary>
        EagerTransferTimeout,

        /// <summary>
        ///     Eager transfer failed.
        /// </summary>
        EagerTransferFailed,

        /// <summary>
        ///     Other unspecified reason.
        /// </summary>
        Other
    }
}