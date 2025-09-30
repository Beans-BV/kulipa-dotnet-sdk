using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Represents the tier or level of a card offering.
    /// </summary>
    public enum CardTier
    {
        /// <summary>
        ///     Standard tier card with basic features.
        /// </summary>
        [JsonPropertyName("standard")] Standard,

        /// <summary>
        ///     Premium tier card with enhanced features and benefits.
        /// </summary>
        [JsonPropertyName("premium")] Premium
    }
}