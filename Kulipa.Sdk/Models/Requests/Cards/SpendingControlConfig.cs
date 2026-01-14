using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Configuration for a spending control.
    ///     Use either <see cref="Period"/> and <see cref="Limit"/> for purchase controls,
    ///     or <see cref="Values"/> for blocked MCC controls.
    /// </summary>
    public sealed record SpendingControlConfig
    {
        /// <summary>
        ///     The time period for the spending limit.
        ///     Used with <see cref="SpendingControlType.Purchase"/> controls.
        /// </summary>
        [JsonPropertyName("period")]
        public SpendingControlPeriod? Period { get; private init; }

        /// <summary>
        ///     The maximum amount allowed per period.
        ///     Used with <see cref="SpendingControlType.Purchase"/> controls.
        /// </summary>
        [JsonPropertyName("limit")]
        public long? Limit { get; private init; }

        /// <summary>
        ///     List of merchant category codes to block.
        ///     Used with <see cref="SpendingControlType.BlockedMcc"/> controls.
        /// </summary>
        [JsonPropertyName("values")]
        public List<string>? Values { get; private init; }

        /// <summary>
        ///     Creates a purchase limit configuration.
        /// </summary>
        /// <param name="period">The time period for the limit.</param>
        /// <param name="limit">The maximum amount allowed per period.</param>
        /// <returns>A new spending control configuration for purchase limits.</returns>
        public static SpendingControlConfig ForPurchaseLimit(SpendingControlPeriod period, long limit)
        {
            return new SpendingControlConfig
            {
                Period = period,
                Limit = limit
            };
        }

        /// <summary>
        ///     Creates a blocked MCC configuration.
        /// </summary>
        /// <param name="mccCodes">The merchant category codes to block.</param>
        /// <returns>A new spending control configuration for blocked MCCs.</returns>
        public static SpendingControlConfig ForBlockedMcc(IEnumerable<string> mccCodes)
        {
            return new SpendingControlConfig
            {
                Values = mccCodes.ToList()
            };
        }
    }
}
