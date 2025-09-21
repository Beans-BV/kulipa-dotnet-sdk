using Kulipa.Sdk.Models.Common;
using Kulipa.Sdk.Models.Users;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Implementation of user operations.
    /// </summary>
    public class UsersResource : BaseResource, IUsersResource
    {
        public UsersResource(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <inheritdoc />
        public async Task<User> CreateAsync(
            CreateUserRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            return await PostAsync<CreateUserRequest, User>(
                "/users",
                request,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> GetAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync<User>(
                $"/users/{userId}",
                userId,
                nameof(userId),
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedResponse<User>> ListAsync(
            string? email = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            var queryParameters = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(email))
            {
                queryParameters["email"] = email;
            }

            return await ListAsync<User>(
                "/users",
                queryParameters,
                pagedRequest,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Balance> GetBalanceAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync<Balance>(
                $"/users/{userId}/balance",
                userId,
                nameof(userId),
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> SetPhoneNumberAsync(
            string userId,
            Phone phone,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, nameof(userId));
            ArgumentNullException.ThrowIfNull(phone);

            return await PutAsync<object, User>(
                $"/users/{userId}/phone-number",
                phone,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> SetEmailAsync(
            string userId,
            string email,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, nameof(userId));
            ArgumentNullException.ThrowIfNull(email);

            return await PutAsync<object, User>(
                $"/users/{userId}/email",
                email,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> SetAddressAsync(
            string userId,
            Address address,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, nameof(userId));
            ArgumentNullException.ThrowIfNull(address);

            return await PutAsync<object, User>(
                $"/users/{userId}/address",
                address,
                idempotencyKey,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> SetShippingAddressAsync(
            string userId,
            ShippingAddress address,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, nameof(userId));
            ArgumentNullException.ThrowIfNull(address);

            return await PutAsync<object, User>(
                $"/users/{userId}/shipping-address",
                address,
                idempotencyKey,
                cancellationToken);
        }
    }
}