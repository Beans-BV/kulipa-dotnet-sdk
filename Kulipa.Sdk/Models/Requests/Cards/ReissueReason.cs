using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Represents the reason for reissuing a card.
    /// </summary>
    public enum ReissueReason
    {
        /// <summary>
        ///     Card was lost.
        /// </summary>
        [JsonPropertyName("lost")] Lost,

        /// <summary>
        ///     Card was stolen from the user.
        /// </summary>
        [JsonPropertyName("stolen")] Stolen,

        /// <summary>
        ///     Card has expired and needs replacement.
        /// </summary>
        [JsonPropertyName("expired")] Expired
    }
}