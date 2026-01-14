using Kulipa.Sdk.Models.Requests.Cards;
using Kulipa.Sdk.Models.Requests.Common;
using Kulipa.Sdk.Models.Responses.Cards;
using Kulipa.Sdk.Models.Responses.Common;

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
        ///     <param name="request">Activate card request containing the activation code.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Card> ActivateAsync(
            string cardId,
            ActivateCardRequest request,
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
        ///     <param name="request">Revoke card request containing the reason.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<Card> RevokeAsync(
            string cardId,
            RevokeCardRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     A POST request to generate a token to view a card's PIN code.
        ///     <param name="cardId">A card identifier.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        /// <returns>Token response containing tokenId and callbackUrl.</returns>
        Task<TokenResponse> GeneratePinCodeTokenAsync(
            string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     A POST request to generate a token to view a card's PAN.
        ///     <param name="cardId">A card identifier.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        /// <returns>Token response containing tokenId and callbackUrl.</returns>
        Task<TokenResponse> GeneratePanTokenAsync(
            string cardId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Creates a replacement card for an existing card that has been lost or stolen.
        ///     The new card will have a new PAN, PIN, expiration date, and CVV2 number.
        ///     Only virtual and physical cards can be reissued.
        /// </summary>
        /// <param name="cardId">A Card identifier.</param>
        /// <param name="request">Reissue card request containing the reason.</param>
        /// <param name="idempotencyKey">Optional idempotency key for the request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The newly issued replacement card.</returns>
        Task<Card> ReissueAsync(
            string cardId,
            ReissueCardRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);
    }
}