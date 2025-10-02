using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Request to activate a card.
    /// </summary>
    public sealed record ActivateCardRequest
    {
        /// <summary>
        ///     Final four numbers of the physical card that is activated for the first time.
        /// </summary>
        [Required]
        [StringLength(4, MinimumLength = 4)]
        [JsonPropertyName("activationCode")]
        public required string ActivationCode { get; init; }
    }
}