using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Represents who initiated the card freeze action.
    /// </summary>
    public enum FrozenBy
    {
        /// <summary>
        ///     Card was frozen by the user.
        /// </summary>
        [JsonPropertyName("user")] User,

        /// <summary>
        ///     Card was frozen by Kulipa (administrative action).
        /// </summary>
        [JsonPropertyName("kulipa")] Kulipa
    }
}