using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Represents the current status of a card.
    /// </summary>
    public enum CardStatus
    {
        /// <summary>
        ///     Card is inactive.
        /// </summary>
        [JsonPropertyName("inactive")] Inactive,

        /// <summary>
        ///     Card is active and can be used for transactions.
        /// </summary>
        [JsonPropertyName("active")] Active,

        /// <summary>
        ///     Card has been cancelled.
        /// </summary>
        [JsonPropertyName("cancelled")] Cancelled,

        /// <summary>
        ///     Card is temporarily frozen and cannot be used.
        /// </summary>
        [JsonPropertyName("frozen")] Frozen,

        /// <summary>
        ///     Card has been reported as lost.
        /// </summary>
        [JsonPropertyName("lost")] Lost,

        /// <summary>
        ///     Card has been reported as stolen.
        /// </summary>
        [JsonPropertyName("stolen")] Stolen,

        /// <summary>
        ///     Card has expired and is no longer valid.
        /// </summary>
        [JsonPropertyName("expired")] Expired
    }
}