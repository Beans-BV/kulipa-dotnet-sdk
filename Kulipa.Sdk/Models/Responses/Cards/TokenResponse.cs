using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Cards
{
    /// <summary>
    ///     Response from generating a PIN code token.
    /// </summary>
    public sealed record TokenResponse
    {
        /// <summary>
        ///     Token identifier for viewing the PIN code.
        /// </summary>
        [JsonPropertyName("tokenId")]
        public string TokenId { get; init; } = null!;

        /// <summary>
        ///     Callback URL for PIN code viewing.
        /// </summary>
        [JsonPropertyName("callbackUrl")]
        public string CallbackUrl { get; init; } = null!;
    }
}