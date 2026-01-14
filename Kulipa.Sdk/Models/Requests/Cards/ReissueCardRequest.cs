using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Request to reissue a card that has been lost or stolen.
    /// </summary>
    public sealed record ReissueCardRequest
    {
        /// <summary>
        ///     Reason for reissuing the card.
        /// </summary>
        [Required]
        [JsonPropertyName("reason")]
        public required ReissueReason Reason { get; init; }
    }
}
