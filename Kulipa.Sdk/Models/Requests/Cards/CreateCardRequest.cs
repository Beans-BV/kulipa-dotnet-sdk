using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kulipa.Sdk.JsonConverters;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Request to create a new card.
    /// </summary>
    public sealed record CreateCardRequest
    {
        /// <summary>
        ///     Card type (physical or virtual).
        /// </summary>
        [Required]
        [JsonPropertyName("type")]
        public CardFormat Format { get; init; }

        /// <summary>
        ///     User identifier. Begins with 'usr-' followed by a v4 UUID.
        /// </summary>
        [Required]
        [JsonPropertyName("userId")]
        public required string UserId { get; init; }

        /// <summary>
        ///     Design identifier. Begins with 'dsn-' followed by a v4 UUID.
        ///     If not supplied, a default value will be used.
        /// </summary>
        [JsonPropertyName("designId")]
        public string? DesignId { get; init; }

        /// <summary>
        ///     Card tier.
        /// </summary>
        [JsonPropertyName("tier")]
        public CardTier Tier { get; init; } = CardTier.Standard;

        /// <summary>
        ///     Currency code. Defaults to USD.
        /// </summary>
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; init; } = "USD";

        /// <summary>
        ///     Reissue from an existing card.
        /// </summary>
        [JsonPropertyName("reissueFromCard")]
        public ReissueFromCard? ReissueFromCard { get; init; }

        /// <summary>
        ///     Delivery type for physical cards (do not set for virtual cards).
        /// </summary>
        [JsonPropertyName("deliveryType")]
        [JsonConverter(typeof(SnakeCaseLowerJsonStringEnumConverter))]
        public DeliveryType? DeliveryType { get; init; }
    }
}