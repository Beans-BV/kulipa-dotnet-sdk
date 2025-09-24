using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Users
{
    /// <summary>
    ///     Represents a physical address.
    /// </summary>
    public class Address
    {
        /// <summary>
        ///     Number and street of the address (e.g., street or company name).
        ///     Maximum length: 150 characters.
        /// </summary>
        [JsonPropertyName("address1")]
        public required string Address1 { get; set; }

        /// <summary>
        ///     Address complementary information (e.g., apartment, place, or building).
        ///     Maximum length: 50 characters.
        /// </summary>
        [JsonPropertyName("address2")]
        public string? Address2 { get; set; }

        /// <summary>
        ///     Postal code or ZIP of the address.
        ///     Maximum length: 16 characters.
        /// </summary>
        [JsonPropertyName("postalCode")]
        public required string PostalCode { get; set; }

        /// <summary>
        ///     City of the address.
        ///     Maximum length: 50 characters.
        /// </summary>
        [JsonPropertyName("city")]
        public required string City { get; set; }

        /// <summary>
        ///     Country code of the address in ISO 3166-1 alpha-2.
        ///     Maximum length: 2 characters.
        /// </summary>
        [JsonPropertyName("country")]
        public required string Country { get; set; }

        /// <summary>
        ///     State, county, province, or region.
        ///     Maximum length: 2 characters.
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}