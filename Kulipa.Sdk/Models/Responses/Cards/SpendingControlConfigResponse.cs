using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Cards
{
    /// <summary>
    ///     Configuration details for a spending control.
    /// </summary>
    public sealed record SpendingControlConfigResponse
    {
        /// <summary>
        ///     The time period for the spending limit.
        ///     Present for <see cref="SpendingControlType.Purchase"/> controls.
        /// </summary>
        [JsonPropertyName("period")]
        public SpendingControlPeriod? Period { get; init; }

        /// <summary>
        ///     The maximum amount allowed per period.
        ///     Present for <see cref="SpendingControlType.Purchase"/> controls.
        /// </summary>
        [JsonPropertyName("limit")]
        public long? Limit { get; init; }

        /// <summary>
        ///     List of merchant category codes that are blocked.
        ///     Present for <see cref="SpendingControlType.BlockedMcc"/> controls.
        /// </summary>
        [JsonPropertyName("values")]
        public List<string>? Values { get; init; }
    }
}
