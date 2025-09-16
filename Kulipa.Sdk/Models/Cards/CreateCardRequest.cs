using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Request to create a new card.
    /// </summary>
    public class CreateCardRequest
    {
        /// <summary>
        ///     Card type (physical or virtual).
        /// </summary>
        [Required]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardType Type { get; set; }

        /// <summary>
        ///     User identifier. Begins with 'usr-' followed by a v4 UUID.
        /// </summary>
        [Required]
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        ///     Design identifier. Begins with 'dsn-' followed by a v4 UUID.
        ///     If not supplied, a default value will be used.
        /// </summary>
        [JsonPropertyName("designId")]
        public string? DesignId { get; set; }

        /// <summary>
        ///     Card tier.
        /// </summary>
        [Required]
        [JsonPropertyName("tier")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardTier Tier { get; set; } = CardTier.Standard;

        /// <summary>
        ///     Currency code. Defaults to USD.
        /// </summary>
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; } = "USD";

        /// <summary>
        ///     Reissue from an existing card.
        /// </summary>
        [JsonPropertyName("reissueFromCard")]
        public ReissueFromCard? ReissueFromCard { get; set; }

        /// <summary>
        ///     Delivery type for physical cards (do not set for virtual cards).
        /// </summary>
        [JsonPropertyName("deliveryType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeliveryType? DeliveryType { get; set; }
    }

    public class ReissueFromCard
    {
        /// <summary>
        ///     Card identifier. Begins with 'crd-' followed by a v4 UUID.
        /// </summary>
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        ///     Reason for reissue.
        /// </summary>
        [Required]
        [JsonPropertyName("reason")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ReissueReason Reason { get; set; }
    }
}