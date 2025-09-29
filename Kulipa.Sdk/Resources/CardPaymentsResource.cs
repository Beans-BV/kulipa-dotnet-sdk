using System.Collections.Specialized;
using Kulipa.Sdk.Models.Enums;
using Kulipa.Sdk.Models.Requests.Common;
using Kulipa.Sdk.Models.Responses.CardPayments;
using Kulipa.Sdk.Models.Responses.Common;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Implementation of card payments.
    /// </summary>
    public class CardPaymentsResource : BaseResource, ICardPaymentsResource
    {
        /// <inheritdoc />
        public CardPaymentsResource(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <inheritdoc />
        public async Task<PagedResponse<CardPayment>> ListAsync(
            string? userId = null,
            string? cardId = null,
            DateTime? updatedAfter = null,
            IEnumerable<CardPaymentStatus>? statuses = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            Action<NameValueCollection> queryBuilder = query =>
            {
                if (updatedAfter.HasValue)
                {
                    query["updatedAfter"] = updatedAfter.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    query["userId"] = userId;
                }

                if (!string.IsNullOrWhiteSpace(cardId))
                {
                    query["cardId"] = cardId;
                }

                if (statuses == null)
                {
                    return;
                }

                // Add status filtering
                var statusList = statuses.ToList();
                if (statusList.Count <= 0)
                {
                    return;
                }

                foreach (var status in statusList)
                {
                    query.Add("statuses", status.ToString().ToLowerInvariant());
                }
            };

            return await ListAsync<CardPayment>(
                "cardPayments",
                queryBuilder,
                pagedRequest,
                cancellationToken);
        }
    }
}