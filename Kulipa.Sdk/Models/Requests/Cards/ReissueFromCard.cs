using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Represents information about a card being reissued from an existing card.
    /// </summary>
    public sealed record ReissueFromCard
    {
        /// <summary>
        ///     Card identifier. Begins with 'crd-' followed by a v4 UUID.
        /// </summary>
        [Required]
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        /// <summary>
        ///     Reason for reissue.
        /// </summary>
        [Required]
        [JsonPropertyName("reason")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required ReissueReason Reason { get; init; }
    }
}