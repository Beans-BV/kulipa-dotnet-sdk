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
        ///     A POST request sent to create a card. Prior to create a card, the user must have completed KyC and verified his
        ///     wallet.
        /// </summary>
        /// <param name="request">Card object that needs to be created/issued for a card owner.</param>
        /// <param name="idempotencyKey">Optional idempotency key for the request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created card.</returns>
        Task<Card> CreateAsync(
            CreateCardRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Fetch a single Card entity.
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
        ///     Activates a card .
        ///     This endpoint must be used for newly created physical cards.
        ///     <param name="cardId">A Card identifier.</param>
        ///     <param name="activationCode">Final four numbers of the physical card that is activated for the first time.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Card> ActivateAsync(
            string cardId,
            string activationCode,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Freezes a card.
        ///     <param name="cardId">A Card identifier.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Card> FreezeAsync(
            string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Unfreezes a previously frozen card to allow new transactions.
        ///     Be aware that even after a successful unfreeze, the card may remain in the frozen status for compliance reasons.
        ///     The frozenBy field will indicate if this is the case.
        ///     <param name="cardId">A Card identifier.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Card> UnfreezeAsync(
            string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     A POST request sent to revoke a card.
        ///     <param name="cardId">A Card identifier.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Card> RevokeAsync(
            string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);
    }
}