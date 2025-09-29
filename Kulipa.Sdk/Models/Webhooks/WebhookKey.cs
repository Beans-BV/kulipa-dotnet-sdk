using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Webhooks
{
    /// <summary>
    ///     Represents a webhook signing key retrieved from Kulipa API.
    /// </summary>
    public class WebhookKey
    {
        /// <summary>
        ///     Gets or sets the unique identifier of the key.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the algorithm used for signing.
        /// </summary>
        [JsonPropertyName("algorithm")]
        public string Algorithm { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the public key value.
        /// </summary>
        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the creation timestamp.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the last update timestamp.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}