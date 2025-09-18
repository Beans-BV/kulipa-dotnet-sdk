using Kulipa.Sdk.Models.Cards;
using Kulipa.Sdk.Models.Common;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Implementation of card operations.
    /// </summary>
    public class CardsResource : BaseResource, ICardsResource
    {
        public CardsResource(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <inheritdoc />
        public async Task<Card> CreateAsync(
            CreateCardRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            return await PostAsync<CreateCardRequest, Card>(
                "/cards",
                request,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Card> GetAsync(
            string cardId,
            CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync<Card>(
                $"/cards/{cardId}",
                cardId,
                nameof(cardId),
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Card>> ListAsync(
            string? userId = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            var queryParameters = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(userId))
            {
                queryParameters["userId"] = userId;
            }

            return await ListAsync<Card>(
                "/cards",
                queryParameters,
                pagedRequest,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Card> ActivateAsync(
            string cardId, string activationCode,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(cardId, nameof(cardId));

            return await PutAsync<Card>(
                $"/cards/{cardId}/activate",
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Card> FreezeAsync(string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(cardId, nameof(cardId));

            return await PutAsync<Card>(
                $"/cards/{cardId}/freeze",
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Card> UnfreezeAsync(
            string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(cardId, nameof(cardId));

            return await PutAsync<Card>(
                $"/cards/{cardId}/unfreeze",
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Card> RevokeAsync(
            string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(cardId, nameof(cardId));

            return await PutAsync<Card>(
                $"/cards/{cardId}/revoke",
                idempotencyKey,
                cancellationToken);
        }
    }
}