using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Represents the physical form factor of a card.
    /// </summary>
    public enum CardFormat
    {
        /// <summary>
        ///     Physical card that can be shipped to the user.
        /// </summary>
        [JsonPropertyName("physical")] Physical,

        /// <summary>
        ///     Virtual card that exists only digitally.
        /// </summary>
        [JsonPropertyName("virtual")] Virtual
    }
}