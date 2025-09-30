using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Kulipa.Sdk.Exceptions;
using Kulipa.Sdk.Models.Common;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Base class for all Kulipa API resource implementations.
    ///     Provides common HTTP operations and error handling patterns.
    /// </summary>
    public abstract class BaseResource
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseResource" /> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making API requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient" /> is null.</exception>
        protected BaseResource(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        ///     Generic GET operation for retrieving a single entity by ID.
        /// </summary>
        /// <typeparam name="T">The entity type to deserialize.</typeparam>
        /// <param name="resourcePath">The API path (e.g., "cards/{id}").</param>
        /// <param name="id">The entity ID.</param>
        /// <param name="idParameterName">The parameter name for validation (e.g., "cardId").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized entity.</returns>
        protected async Task<T> GetByIdAsync<T>(
            string resourcePath,
            string id,
            string idParameterName,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, idParameterName);

            var response = await _httpClient.GetAsync(resourcePath, cancellationToken);
            await EnsureSuccessStatusCode(response);

            return await DeserializeResponse<T>(response, cancellationToken);
        }

        /// <summary>
        ///     Generic POST operation for creating entities with optional idempotency.
        /// </summary>
        /// <typeparam name="TRequest">The request model type.</typeparam>
        /// <typeparam name="TResponse">The response model type.</typeparam>
        /// <param name="resourcePath">The API path (e.g., "cards").</param>
        /// <param name="request">The request payload.</param>
        /// <param name="idempotencyKey">Optional idempotency key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created entity.</returns>
        protected async Task<TResponse> PostAsync<TRequest, TResponse>(
            string resourcePath,
            TRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, resourcePath)
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };

            AddIdempotencyKey(httpRequest, idempotencyKey);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            await EnsureSuccessStatusCode(response);

            return await DeserializeResponse<TResponse>(response, cancellationToken);
        }

        /// <summary>
        ///     Generic PUT operation for updating entities with optional idempotency.
        /// </summary>
        /// <typeparam name="TRequest">The request model type.</typeparam>
        /// <typeparam name="TResponse">The response model type.</typeparam>
        /// <param name="resourcePath">The API path (e.g., "cards/{id}/freeze").</param>
        /// <param name="request">The request payload (can be null for simple operations).</param>
        /// <param name="idempotencyKey">Optional idempotency key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated entity.</returns>
        protected async Task<TResponse> PutAsync<TRequest, TResponse>(
            string resourcePath,
            TRequest? request = default,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, resourcePath);

            if (request != null)
            {
                httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
            }

            AddIdempotencyKey(httpRequest, idempotencyKey);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            await EnsureSuccessStatusCode(response);

            return await DeserializeResponse<TResponse>(response, cancellationToken);
        }

        /// <summary>
        ///     Generic PUT operation for simple updates with no request body.
        /// </summary>
        /// <typeparam name="TResponse">The response model type.</typeparam>
        /// <param name="resourcePath">The API path (e.g., "cards/{id}/freeze").</param>
        /// <param name="idempotencyKey">Optional idempotency key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated entity.</returns>
        protected async Task<TResponse> PutAsync<TResponse>(
            string resourcePath,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            return await PutAsync<object, TResponse>(resourcePath, null, idempotencyKey, cancellationToken);
        }

        /// <summary>
        ///     Generic list operation with pagination support.
        /// </summary>
        /// <typeparam name="T">The entity type in the paged response.</typeparam>
        /// <param name="resourcePath">The base API path (e.g., "cards").</param>
        /// <param name="queryParameters">Additional query parameters.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged response containing the entities.</returns>
        protected async Task<PagedResponse<T>> ListAsync<T>(
            string resourcePath,
            Dictionary<string, string?>? queryParameters = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            pagedRequest ??= new PagedRequest();
            var query = HttpUtility.ParseQueryString(string.Empty);

            // Add custom query parameters
            if (queryParameters != null)
            {
                foreach (var (key, value) in queryParameters)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        query[key] = value;
                    }
                }
            }

            // Add pagination parameters
            query["limit"] = Math.Min(Math.Max(pagedRequest.Limit, 1), 100).ToString();
            query["fromPage"] = Math.Max(pagedRequest.FromPage, 0).ToString();
            if (!string.IsNullOrEmpty(pagedRequest.SortBy))
            {
                query["sortBy"] = pagedRequest.SortBy;
            }

            var queryString = query.ToString();
            var url = string.IsNullOrEmpty(queryString) ? resourcePath : $"{resourcePath}?{queryString}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            await EnsureSuccessStatusCode(response);

            return await DeserializeResponse<PagedResponse<T>>(response, cancellationToken);
        }

        /// <summary>
        ///     Generic list operation with advanced query parameter support (for arrays, dates, etc.).
        /// </summary>
        /// <typeparam name="T">The entity type in the paged response.</typeparam>
        /// <param name="resourcePath">The base API path (e.g., "cards").</param>
        /// <param name="queryBuilder">Function to build query parameters.</param>
        /// <param name="pagedRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged response containing the entities.</returns>
        protected async Task<PagedResponse<T>> ListAsync<T>(
            string resourcePath,
            Action<NameValueCollection> queryBuilder,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            pagedRequest ??= new PagedRequest();
            var query = HttpUtility.ParseQueryString(string.Empty);

            // Let the caller build custom query parameters
            queryBuilder(query);

            // Add pagination parameters
            query["limit"] = Math.Min(Math.Max(pagedRequest.Limit, 1), 100).ToString();
            query["fromPage"] = Math.Max(pagedRequest.FromPage, 0).ToString();
            if (!string.IsNullOrEmpty(pagedRequest.SortBy))
            {
                query["sortBy"] = pagedRequest.SortBy;
            }

            var queryString = query.ToString();
            var url = string.IsNullOrEmpty(queryString) ? resourcePath : $"{resourcePath}?{queryString}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            await EnsureSuccessStatusCode(response);

            return await DeserializeResponse<PagedResponse<T>>(response, cancellationToken);
        }

        /// <summary>
        ///     Validates that an ID parameter is not null or empty.
        /// </summary>
        /// <param name="id">The ID to validate.</param>
        /// <param name="parameterName">The parameter name for error messages.</param>
        /// <exception cref="ArgumentException">Thrown when ID is null or empty.</exception>
        protected static void ValidateId(string id, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"{parameterName} cannot be null or empty", parameterName);
            }
        }

        /// <summary>
        ///     Adds idempotency key to the request if provided.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="idempotencyKey">The idempotency key.</param>
        private static void AddIdempotencyKey(HttpRequestMessage request, string? idempotencyKey)
        {
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                request.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), idempotencyKey);
            }
        }

        /// <summary>
        ///     Ensures the response has a success status code, otherwise handles the error.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }
        }

        /// <summary>
        ///     Deserializes the response content to the specified type with null checking.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="response">The HTTP response message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="KulipaException">Thrown when deserialization fails.</exception>
        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var entity = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);

            if (entity == null)
            {
                throw new KulipaException($"Failed to deserialize {typeof(T).Name} response");
            }

            return entity;
        }

        /// <summary>
        ///     Handles error responses by throwing appropriate Kulipa exceptions.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            var requestId = response.Headers.TryGetValues("x-request-id", out var values)
                ? values.FirstOrDefault()
                : null;

            throw response.StatusCode switch
            {
                HttpStatusCode.BadRequest => new KulipaValidationException("Invalid request", content, requestId),
                HttpStatusCode.Unauthorized => new KulipaAuthenticationException("Authentication failed", requestId),
                HttpStatusCode.Forbidden => new KulipaAuthorizationException("Access forbidden", requestId),
                HttpStatusCode.TooManyRequests => new KulipaRateLimitException("Rate limit exceeded", 0,
                    DateTime.UtcNow.AddMinutes(1), 60),
                HttpStatusCode.InternalServerError => new KulipaServerException("Internal server error", requestId,
                    HttpStatusCode.InternalServerError),
                HttpStatusCode.BadGateway => new KulipaServerException("Bad gateway", requestId,
                    HttpStatusCode.BadGateway),
                HttpStatusCode.ServiceUnavailable => new KulipaServerException("Service unavailable", requestId,
                    HttpStatusCode.ServiceUnavailable),
                HttpStatusCode.GatewayTimeout => new KulipaServerException("Gateway timeout", requestId,
                    HttpStatusCode.GatewayTimeout),
                _ => new KulipaApiException($"API request failed with status {response.StatusCode}", content,
                    requestId, response.StatusCode)
            };
        }
    }
}