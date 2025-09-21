using Kulipa.Sdk.Models.Common;
using Kulipa.Sdk.Models.Users;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Interface for users.
    /// </summary>
    public interface IUsersResource
    {
        /// <summary>
        ///     A POST request sent to create a User and Wallet.
        ///     This endpoint is common to both prepaid and debit programs.
        /// </summary>
        /// <param name="request">Data to create a new user with a wallet.</param>
        /// <param name="idempotencyKey">Optional idempotency key for the request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created user.</returns>
        Task<User> CreateAsync(
            CreateUserRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Retrieve a User entity via its ID.
        /// </summary>
        /// <param name="userId">The UUID of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user details.</returns>
        Task<User> GetAsync(
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Lists users with optional filtering.
        /// </summary>
        /// <param name="email">Optional email of the cardholder user to filter by.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged list of users.</returns>
        Task<PagedResponse<User>> ListAsync(
            string? email = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Retrieve a user balance via its ID.
        /// </summary>
        /// <param name="userId">The UUID of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user balance details.</returns>
        Task<Balance> GetBalanceAsync(
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Set the phone number of the specified user identified by its ID.
        ///     <param name="userId">The UUID of the user.</param>
        ///     <param name="phone">Phone number to be set.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<User> SetPhoneNumberAsync(
            string userId,
            Phone phone,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Update the email address of the specified user identified by its ID.
        ///     <param name="userId">The UUID of the user.</param>
        ///     <param name="email">Email address to be set for the user.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<User> SetEmailAsync(
            string userId,
            string email,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Update the address of the specified user identified by its ID.
        ///     Only users who are exempt from KYC can update their address directly.
        ///     Users who have undergone KYC must complete a new KYC process to update their address.
        ///     <param name="userId">The UUID of the user.</param>
        ///     <param name="address">Address to be set for the user.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<User> SetAddressAsync(
            string userId,
            Address address,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        ///     Update the shipping address of the specified user identified by its ID.
        ///     <param name="userId">The UUID of the user.</param>
        ///     <param name="shippingAddress">Address to be set for the user.</param>
        ///     <param name="idempotencyKey">Optional idempotency key for the request.</param>
        ///     <param name="cancellationToken">Cancellation token.</param>
        /// </summary>
        Task<User> SetShippingAddressAsync(
            string userId,
            ShippingAddress shippingAddress,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);
    }
}