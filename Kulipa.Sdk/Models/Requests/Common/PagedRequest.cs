namespace Kulipa.Sdk.Models.Requests.Common
{
    /// <summary>
    ///     Query parameters for paged requests.
    /// </summary>
    public sealed record PagedRequest
    {
        /// <summary>
        ///     A limit on the number of objects returned (1-100).
        /// </summary>
        public int Limit { get; init; } = 10;

        /// <summary>
        ///     Designates the start page in the sort order from which data should be retrieved.
        /// </summary>
        public int FromPage { get; init; } = 0;

        /// <summary>
        ///     Designates the field to sort on.
        ///     There is a limited set of fields to sort on.
        ///     Preface the field name with a hyphen (-) to sort in descending order.
        ///     Exclude the hyphen to sort in ascending order.
        /// </summary>
        public string SortBy { get; init; } = "-createdAt";
    }
}