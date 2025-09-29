using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Users
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
        [Required]
        [JsonPropertyName("address1")]
        public string Address1 { get; set; } = null!;

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
        [Required]
        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; } = null!;

        /// <summary>
        ///     City of the address.
        ///     Maximum length: 50 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("city")]
        public string City { get; set; } = null!;

        /// <summary>
        ///     Country code of the address in ISO 3166-1 alpha-2.
        ///     Maximum length: 2 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("country")]
        public string Country { get; set; } = null!;

        /// <summary>
        ///     State, county, province, or region.
        ///     Maximum length: 2 characters.
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}