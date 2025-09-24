using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Represents card expiration information.
    /// </summary>
    public class CardExpiration
    {
        /// <summary>
        ///     Gets or sets the expiration month (1-12).
        /// </summary>
        [JsonPropertyName("month")]
        public int Month { get; set; }

        /// <summary>
        ///     Gets or sets the expiration year (e.g., 2025).
        /// </summary>
        [JsonPropertyName("year")]
        public int Year { get; set; }
    }
}