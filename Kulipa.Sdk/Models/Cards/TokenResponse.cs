using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Response from generating a PIN code token.
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        ///     Token identifier for viewing the PIN code.
        /// </summary>
        [JsonPropertyName("tokenId")]
        public required string TokenId { get; set; }

        /// <summary>
        ///     Callback URL for PIN code viewing.
        /// </summary>
        [JsonPropertyName("callbackUrl")]
        public required string CallbackUrl { get; set; }
    }
}