using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Cards
{
    /// <summary>
    ///     Represents the name information to be embossed on a physical card.
    /// </summary>
    public sealed record EmbossedName
    {
        /// <summary>
        ///     Gets the first line of the printed name on the card.
        /// </summary>
        [JsonPropertyName("printedName1")]
        public string PrintedName1 { get; init; } = null!;

        /// <summary>
        ///     Gets the second line of the printed name on the card.
        /// </summary>
        [JsonPropertyName("printedName2")]
        public string PrintedName2 { get; init; } = null!;
    }
}