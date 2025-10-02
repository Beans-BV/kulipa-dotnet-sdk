using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Request to revoke a card.
    /// </summary>
    public sealed record RevokeCardRequest
    {
        /// <summary>
        ///     Reason for revoking the card.
        /// </summary>
        [Required]
        [JsonPropertyName("reason")]
        public required RevokeReason Reason { get; init; }
    }
}
