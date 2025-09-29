using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Common
{
    /// <summary>
    ///     Represents a paged response from the API.
    /// </summary>
    /// <typeparam name="T">The type of items in the response.</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        ///     The index of the page from the result.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        ///     The number of items returned.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        ///     Indicates if more elements exist beyond this set.
        /// </summary>
        [JsonPropertyName("hasMore")]
        public bool HasMore { get; set; }

        /// <summary>
        ///     The items in this page.
        /// </summary>
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = [];
    }
}