using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Cards
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
        public string TokenId { get; set; } = null!;

        /// <summary>
        ///     Callback URL for PIN code viewing.
        /// </summary>
        [JsonPropertyName("callbackUrl")]
        public string CallbackUrl { get; set; } = null!;
    }
}