using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Cards
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
        public string Id { get; set; } = null!;

        /// <summary>
        ///     The UUID of the card owner.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = null!;

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
        public CardFormat Format { get; set; }

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
        public string LastFourDigits { get; set; } = null!;

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
        public CardExpiration Expiration { get; set; } = null!;

        /// <summary>
        ///     Embossed name on the card.
        /// </summary>
        [JsonPropertyName("embossedName")]
        public EmbossedName EmbossedName { get; set; } = null!;

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
}