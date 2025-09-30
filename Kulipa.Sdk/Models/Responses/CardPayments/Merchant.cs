using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.CardPayments
{
    /// <summary>
    ///     Represents merchant information for card payments.
    /// </summary>
    public sealed record Merchant
    {
        /// <summary>
        ///     The merchant's business name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>
        ///     The city where the merchant is located.
        /// </summary>
        [JsonPropertyName("city")]
        public required string City { get; init; }

        /// <summary>
        ///     The country where the merchant is located, in ISO 3166-1 alpha-2 format.
        /// </summary>
        [JsonPropertyName("country")]
        public required string Country { get; init; }

        /// <summary>
        ///     Merchant Category Code (MCC) identifying the type of goods or services provided by the merchant.
        /// </summary>
        [JsonPropertyName("mcc")]
        public string? Mcc { get; init; }

        /// <summary>
        ///     Merchant Identifier (MID) used to uniquely identify the specific merchant.
        /// </summary>
        [JsonPropertyName("mid")]
        public string? Mid { get; init; }
    }
}