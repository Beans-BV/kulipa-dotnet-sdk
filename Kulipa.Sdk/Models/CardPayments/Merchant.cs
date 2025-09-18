using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.CardPayments
{
    /// <summary>
    ///     Represents merchant information for card payments.
    /// </summary>
    public class Merchant
    {
        /// <summary>
        ///     The merchant's business name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        ///     The city where the merchant is located.
        /// </summary>
        [JsonPropertyName("city")]
        public required string City { get; set; }

        /// <summary>
        ///     The country where the merchant is located, in ISO 3166-1 alpha-2 format.
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        /// <summary>
        ///     Merchant Category Code (MCC) identifying the type of goods or services provided by the merchant.
        /// </summary>
        [JsonPropertyName("mcc")]
        public string? Mcc { get; set; }

        /// <summary>
        ///     Merchant Identifier (MID) used to uniquely identify the specific merchant.
        /// </summary>
        [JsonPropertyName("mid")]
        public string? Mid { get; set; }
    }
}