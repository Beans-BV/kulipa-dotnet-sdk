using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Cards
{
    /// <summary>
    ///     Represents a Kulipa card.
    /// </summary>
    public sealed record Card
    {
        /// <summary>
        ///     Card identifier. Begins with 'crd-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        /// <summary>
        ///     The UUID of the card owner.
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; init; }

        /// <summary>
        ///     Currency code (e.g., USD).
        /// </summary>
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; init; } = "USD";

        /// <summary>
        ///     The UUID of the card's design.
        /// </summary>
        [JsonPropertyName("designId")]
        public string? DesignId { get; init; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; init; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; init; }

        /// <summary>
        ///     Card type.
        /// </summary>
        [JsonPropertyName("type")]
        public CardType Type { get; init; }

        /// <summary>
        ///     Card status.
        /// </summary>
        [JsonPropertyName("status")]
        public CardStatus Status { get; init; }

        /// <summary>
        ///     Last 4 digits of the card number.
        /// </summary>
        [JsonPropertyName("lastFourDigits")]
        public required string LastFourDigits { get; init; }

        /// <summary>
        ///     Whether contactless payments are enabled.
        /// </summary>
        [JsonPropertyName("contactlessEnabled")]
        public bool ContactlessEnabled { get; init; }

        /// <summary>
        ///     Whether ATM withdrawals are enabled.
        /// </summary>
        [JsonPropertyName("withdrawalEnabled")]
        public bool WithdrawalEnabled { get; init; }

        /// <summary>
        ///     Whether internet purchases are enabled.
        /// </summary>
        [JsonPropertyName("internetPurchaseEnabled")]
        public bool InternetPurchaseEnabled { get; init; }

        /// <summary>
        ///     Card expiration details.
        /// </summary>
        [JsonPropertyName("expiration")]
        public required CardExpiration Expiration { get; init; }

        /// <summary>
        ///     Embossed name on the card.
        /// </summary>
        [JsonPropertyName("embossedName")]
        public required EmbossedName EmbossedName { get; init; }

        /// <summary>
        ///     Who froze the card (only present when status is "frozen").
        /// </summary>
        [JsonPropertyName("frozenBy")]
        public FrozenBy? FrozenBy { get; init; }

        /// <summary>
        ///     Card tier.
        /// </summary>
        [JsonPropertyName("tier")]
        public CardTier Tier { get; init; }
    }
}