using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Represents the name information to be embossed on a physical card.
    /// </summary>
    public class EmbossedName
    {
        /// <summary>
        ///     Gets or sets the first line of the printed name on the card.
        /// </summary>
        [JsonPropertyName("printedName1")] public string? PrintedName1 { get; set; }

        /// <summary>
        ///     Gets or sets the second line of the printed name on the card.
        /// </summary>
        [JsonPropertyName("printedName2")] public string? PrintedName2 { get; set; }
    }
}