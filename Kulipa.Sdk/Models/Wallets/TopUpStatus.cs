using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Possible statuses of a top-up.
    /// </summary>
    public enum TopUpStatus
    {
        [JsonPropertyName("confirmed")] Confirmed,
        [JsonPropertyName("failed")] Failed
    }
}