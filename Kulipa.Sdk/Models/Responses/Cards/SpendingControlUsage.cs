using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Cards
{
    /// <summary>
    ///     Represents a spending control with its current usage on a card.
    /// </summary>
    public sealed record SpendingControlUsage
    {
        /// <summary>
        ///     Spending control identifier. Begins with 'scl-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        /// <summary>
        ///     Human-readable explanation of this spending control.
        /// </summary>
        [JsonPropertyName("description")]
        public required string Description { get; init; }

        /// <summary>
        ///     Type of spending control.
        /// </summary>
        [JsonPropertyName("type")]
        public SpendingControlType Type { get; init; }

        /// <summary>
        ///     Configuration for the spending control.
        /// </summary>
        [JsonPropertyName("config")]
        public required SpendingControlConfigResponse Config { get; init; }

        /// <summary>
        ///     Remaining budget amount for the current period.
        ///     Only present for <see cref="SpendingControlType.Purchase"/> controls.
        /// </summary>
        [JsonPropertyName("availableAmount")]
        public long? AvailableAmount { get; init; }
    }
}
