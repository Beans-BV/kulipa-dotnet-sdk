using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Request to create a spending control on a card.
    /// </summary>
    public sealed record CreateSpendingControlRequest
    {
        /// <summary>
        ///     Human-readable explanation of this spending control.
        /// </summary>
        [Required]
        [JsonPropertyName("description")]
        public required string Description { get; init; }

        /// <summary>
        ///     Type of spending control.
        /// </summary>
        [Required]
        [JsonPropertyName("type")]
        public SpendingControlType Type { get; init; }

        /// <summary>
        ///     Configuration for the spending control.
        ///     Use <see cref="SpendingControlConfig.ForPurchaseLimit"/> for purchase limits,
        ///     or <see cref="SpendingControlConfig.ForBlockedMcc"/> for blocked merchant categories.
        /// </summary>
        [Required]
        [JsonPropertyName("config")]
        public required SpendingControlConfig Config { get; init; }
    }
}
