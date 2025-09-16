using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Kulipa.Sdk.Exceptions;
using Kulipa.Sdk.Models.Cards;
using Kulipa.Sdk.Models.Common;

namespace Kulipa.Sdk.Resources
{
    /// <summary>
    ///     Implementation of card operations.
    /// </summary>
    public class CardsResource : ICardsResource
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public CardsResource(HttpClient httpClient, ILogger<CardsResource> logger)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Card> CreateAsync(
            CreateCardRequest request,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/cards")
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };

            // Add idempotency key if provided
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                httpRequest.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), idempotencyKey);
            }

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }

            var card = await response.Content.ReadFromJsonAsync<Card>(_jsonOptions, cancellationToken);

            if (card == null)
            {
                throw new KulipaException("Failed to deserialize card response");
            }

            return card;
        }

        public async Task<Card> GetAsync(string cardId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cardId))
            {
                throw new ArgumentException("Card ID cannot be null or empty", nameof(cardId));
            }

            var response = await _httpClient.GetAsync($"/cards/{cardId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }

            var card = await response.Content.ReadFromJsonAsync<Card>(_jsonOptions, cancellationToken);

            if (card == null)
            {
                throw new KulipaException("Failed to deserialize card response");
            }

            return card;
        }

        public async Task<PagedResponse<Card>> ListAsync(
            string? userId = null,
            PagedRequest? pagedRequest = null,
            CancellationToken cancellationToken = default)
        {
            pagedRequest ??= new PagedRequest();

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(userId))
            {
                query["userId"] = userId;
            }

            query["limit"] = Math.Min(Math.Max(pagedRequest.Limit, 1), 100).ToString();
            query["fromPage"] = Math.Max(pagedRequest.FromPage, 0).ToString();
            query["sortBy"] = pagedRequest.SortBy;

            var queryString = query.ToString();
            var url = string.IsNullOrEmpty(queryString) ? "/cards" : $"/cards?{queryString}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }

            var pagedResponse =
                await response.Content.ReadFromJsonAsync<PagedResponse<Card>>(_jsonOptions, cancellationToken);

            if (pagedResponse == null)
            {
                throw new KulipaException("Failed to deserialize paged response");
            }

            return pagedResponse;
        }

        public async Task<Card> FreezeAsync(string cardId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(cardId))
            {
                throw new ArgumentException("Card ID cannot be null or empty", nameof(cardId));
            }

            var response = await _httpClient.PostAsync($"/cards/{cardId}/freeze", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }

            var card = await response.Content.ReadFromJsonAsync<Card>(_jsonOptions, cancellationToken);

            if (card == null)
            {
                throw new KulipaException("Failed to deserialize card response");
            }

            return card;
        }

        public async Task<Card> UnfreezeAsync(string cardId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(cardId))
            {
                throw new ArgumentException("Card ID cannot be null or empty", nameof(cardId));
            }

            var response = await _httpClient.PostAsync($"/cards/{cardId}/unfreeze", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }

            var card = await response.Content.ReadFromJsonAsync<Card>(_jsonOptions, cancellationToken);

            if (card == null)
            {
                throw new KulipaException("Failed to deserialize card response");
            }

            return card;
        }

        public async Task<Card> CancelAsync(string cardId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(cardId))
            {
                throw new ArgumentException("Card ID cannot be null or empty", nameof(cardId));
            }

            var response = await _httpClient.PostAsync($"/cards/{cardId}/cancel", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }

            var card = await response.Content.ReadFromJsonAsync<Card>(_jsonOptions, cancellationToken);

            if (card == null)
            {
                throw new KulipaException("Failed to deserialize card response");
            }

            return card;
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            var requestId = response.Headers.TryGetValues("x-request-id", out var values)
                ? values.FirstOrDefault()
                : null;

            throw response.StatusCode switch
            {
                HttpStatusCode.BadRequest => new KulipaValidationException("Invalid request", content,
                    requestId),
                HttpStatusCode.Unauthorized => new KulipaAuthenticationException("Authentication failed",
                    requestId),
                HttpStatusCode.Forbidden => new KulipaAuthorizationException("Access forbidden", requestId),
                HttpStatusCode.TooManyRequests => new KulipaRateLimitException("Rate limit exceeded", 0,
                    DateTime.UtcNow.AddMinutes(1), 60),
                HttpStatusCode.InternalServerError => new KulipaServerException("Internal server error",
                    requestId, HttpStatusCode.InternalServerError),
                HttpStatusCode.BadGateway => new KulipaServerException("Bad gateway", requestId,
                    HttpStatusCode.BadGateway),
                HttpStatusCode.ServiceUnavailable => new KulipaServerException("Service unavailable",
                    requestId, HttpStatusCode.ServiceUnavailable),
                HttpStatusCode.GatewayTimeout => new KulipaServerException("Gateway timeout", requestId,
                    HttpStatusCode.GatewayTimeout),
                _ => new KulipaApiException($"API request failed with status {response.StatusCode}", content, requestId, response.StatusCode)
            };
        }
    }
}