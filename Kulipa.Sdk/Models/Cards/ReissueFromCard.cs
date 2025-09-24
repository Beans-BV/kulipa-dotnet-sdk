using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Represents information about a card being reissued from an existing card.
    /// </summary>
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