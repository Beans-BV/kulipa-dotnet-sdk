using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Possible statuses of a top-up.
    /// </summary>
    public enum TopUpStatus
    {
        /// <summary>
        ///     The top-up transaction has been confirmed and successfully processed.
        /// </summary>
        [JsonPropertyName("confirmed")] Confirmed,

        /// <summary>
        ///     The top-up transaction has failed and was not processed.
        /// </summary>
        [JsonPropertyName("failed")] Failed
    }
}