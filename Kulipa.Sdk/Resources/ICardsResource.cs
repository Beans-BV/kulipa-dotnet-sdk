using Kulipa.Sdk.Models.Cards;
using Kulipa.Sdk.Models.Common;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Interface for card operations.
    /// </summary>
    public interface ICardsResource
    {
        /// <summary>
        ///     Creates a new card.
        /// </summary>
        /// <param name="request">Card creation request.</param>
        /// <param name="idempotencyKey">Optional idempotency key for the request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created card.</returns>
        Task<Card> CreateAsync(
            CreateCardRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Retrieves a card by ID.
        /// </summary>
        /// <param name="cardId">Card identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The card details.</returns>
        Task<Card> GetAsync(
            string cardId,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Lists cards with optional filtering.
        /// </summary>
        /// <param name="userId">Optional user ID to filter by.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged list of cards.</returns>
        Task<PagedResponse<Card>> ListAsync(
            string? userId = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Freezes a card.
        /// </summary>
        Task<Card> FreezeAsync(
            string cardId,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Unfreezes a card.
        /// </summary>
        Task<Card> UnfreezeAsync(
            string cardId,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Cancels a card.
        /// </summary>
        Task<Card> CancelAsync(
            string cardId,
            CancellationToken cancellationToken = default);
    }
}