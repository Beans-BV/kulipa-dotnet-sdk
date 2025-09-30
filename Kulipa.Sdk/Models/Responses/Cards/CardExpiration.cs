using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Cards
{
    /// <summary>
    ///     Represents card expiration information.
    /// </summary>
    public sealed record CardExpiration
    {
        /// <summary>
        ///     Gets the expiration month (1-12).
        /// </summary>
        [JsonPropertyName("month")]
        public int Month { get; init; }

        /// <summary>
        ///     Gets the expiration year (e.g., 2025).
        /// </summary>
        [JsonPropertyName("year")]
        public int Year { get; init; }
    }
}