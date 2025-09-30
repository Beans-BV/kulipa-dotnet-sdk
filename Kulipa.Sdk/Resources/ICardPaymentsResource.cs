using Kulipa.Sdk.Models.Enums;
using Kulipa.Sdk.Models.Requests.Common;
using Kulipa.Sdk.Models.Responses.CardPayments;
using Kulipa.Sdk.Models.Responses.Common;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Interface for card payments.
    /// </summary>
    public interface ICardPaymentsResource
    {
        /// <summary>
        ///     A GET request to list card payments.
        /// </summary>
        /// <param name="userId">Optional user ID to filter by.</param>
        /// <param name="cardId">Optional card ID to filter by.</param>
        /// <param name="updatedAfter">Filter payments updated after this date and time.</param>
        /// <param name="statuses">Filter payments by status. Multiple statuses can be specified.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged list of cards.</returns>
        Task<PagedResponse<CardPayment>> ListAsync(
            string? userId = null,
            string? cardId = null,
            DateTime? updatedAfter = null,
            IEnumerable<CardPaymentStatus>? statuses = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default);
    }
}