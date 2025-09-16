using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Represents a Kulipa card.
    /// </summary>
    public class Card
    {
        /// <summary>
        ///     Card identifier. Begins with 'crd-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        ///     The UUID of the card owner.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        ///     Currency code (e.g., USD).
        /// </summary>
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; } = "USD";

        /// <summary>
        ///     The UUID of the card's design.
        /// </summary>
        [JsonPropertyName("designId")]
        public string? DesignId { get; set; }

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

        /// <summary>
        ///     Card type.
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardType Type { get; set; }

        /// <summary>
        ///     Card status.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardStatus Status { get; set; }

        /// <summary>
        ///     Last 4 digits of the card number.
        /// </summary>
        [JsonPropertyName("lastFourDigits")]
        public string LastFourDigits { get; set; } = string.Empty;

        /// <summary>
        ///     Whether contactless payments are enabled.
        /// </summary>
        [JsonPropertyName("contactlessEnabled")]
        public bool ContactlessEnabled { get; set; }

        /// <summary>
        ///     Whether ATM withdrawals are enabled.
        /// </summary>
        [JsonPropertyName("withdrawalEnabled")]
        public bool WithdrawalEnabled { get; set; }

        /// <summary>
        ///     Whether internet purchases are enabled.
        /// </summary>
        [JsonPropertyName("internetPurchaseEnabled")]
        public bool InternetPurchaseEnabled { get; set; }

        /// <summary>
        ///     Card expiration details.
        /// </summary>
        [JsonPropertyName("expiration")]
        public CardExpiration? Expiration { get; set; }

        /// <summary>
        ///     Embossed name on the card.
        /// </summary>
        [JsonPropertyName("embossedName")]
        public EmbossedName? EmbossedName { get; set; }

        /// <summary>
        ///     Who froze the card (only present when status is "frozen").
        /// </summary>
        [JsonPropertyName("frozenBy")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FrozenBy? FrozenBy { get; set; }

        /// <summary>
        ///     Card tier.
        /// </summary>
        [JsonPropertyName("tier")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardTier Tier { get; set; }
    }

    public enum CardType
    {
        [JsonPropertyName("physical")] Physical,
        [JsonPropertyName("virtual")] Virtual
    }

    public enum CardStatus
    {
        [JsonPropertyName("inactive")] Inactive,
        [JsonPropertyName("active")] Active,
        [JsonPropertyName("cancelled")] Cancelled,
        [JsonPropertyName("frozen")] Frozen,
        [JsonPropertyName("lost")] Lost,
        [JsonPropertyName("stolen")] Stolen,
        [JsonPropertyName("expired")] Expired
    }

    public enum CardTier
    {
        [JsonPropertyName("standard")] Standard,
        [JsonPropertyName("premium")] Premium
    }

    public enum FrozenBy
    {
        [JsonPropertyName("user")] User,
        [JsonPropertyName("kulipa")] Kulipa
    }

    public enum ReissueReason
    {
        [JsonPropertyName("lost")] Lost,
        [JsonPropertyName("stolen")] Stolen,
        [JsonPropertyName("expired")] Expired
    }

    public enum DeliveryType
    {
        [JsonPropertyName("ship_to_user")] ShipToUser
    }

    public class CardExpiration
    {
        [JsonPropertyName("month")] public int Month { get; set; }

        [JsonPropertyName("year")] public int Year { get; set; }
    }

    public class EmbossedName
    {
        [JsonPropertyName("printedName1")] public string? PrintedName1 { get; set; }

        [JsonPropertyName("printedName2")] public string? PrintedName2 { get; set; }
    }
}