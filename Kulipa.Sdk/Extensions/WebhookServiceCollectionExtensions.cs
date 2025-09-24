using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Core;
using Kulipa.Sdk.Resources;
using Kulipa.Sdk.Webhooks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Kulipa.Sdk.Extensions
{
    /// <summary>
    ///     Extension methods for registering Kulipa webhook services.
    /// </summary>
    public static class WebhookServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds Kulipa webhook services to the service collection.
        ///     This should be called as part of AddKulipaSdk().
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        internal static IServiceCollection AddKulipaWebhookServices(this IServiceCollection services)
        {
            // Register webhook verification services
            services.AddSingleton<IPublicKeyCache>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                // Reuse HTTP client
                var httpClient = httpClientFactory.CreateClient(nameof(KulipaClient));
                var logger = serviceProvider.GetRequiredService<ILogger<MemoryPublicKeyCache>>();
                var options = serviceProvider.GetRequiredService<IOptions<KulipaSdkOptions>>();
                return new MemoryPublicKeyCache(httpClient, logger, options);
            });

            services.TryAddSingleton<IWebhookVerifier, WebhookVerifier>();
            services.TryAddScoped<IWebhooksResource, WebhooksResource>();

            return services;
        }
    }
}