using System.Net;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Core;
using Kulipa.Sdk.Services.Authentication;
using Kulipa.Sdk.Services.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Kulipa.Sdk.Extensions
{
    /// <summary>
    ///     Extension methods for IServiceCollection to register Kulipa SDK services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the Kulipa SDK services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="sectionName">The configuration section name containing Kulipa settings.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddKulipaSdk(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "Kulipa")
        {
            services.Configure<KulipaSdkOptions>(configuration.GetSection(sectionName));
            return services.AddKulipaSdkCore();
        }

        /// <summary>
        ///     Adds Kulipa SDK services with explicit options.
        /// </summary>
        public static IServiceCollection AddKulipaSdk(
            this IServiceCollection services,
            Action<KulipaSdkOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddKulipaSdkCore();
        }

        private static IServiceCollection AddKulipaSdkCore(this IServiceCollection services)
        {
            // Validate options on startup
            services.AddSingleton<IValidateOptions<KulipaSdkOptions>, KulipaSdkOptionsValidator>();

            // Register message handlers
            services.AddTransient<ApiKeyAuthenticationHandler>();
            services.AddTransient<RateLimitHandler>();
            services.AddTransient<IdempotencyHandler>();

            // Register HTTP client for the main client
            services.AddHttpClient<IKulipaClient, KulipaClient>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<KulipaSdkOptions>>();
                    ConfigureHttpClient(client, options.Value);
                })
                .AddHttpMessageHandler<ApiKeyAuthenticationHandler>()
                .AddHttpMessageHandler<IdempotencyHandler>()
                .AddHttpMessageHandler<RateLimitHandler>()
                .AddPolicyHandler((serviceProvider, request) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<KulipaSdkOptions>>().Value.RetryPolicy;
                    var logger = serviceProvider.GetRequiredService<ILogger<KulipaClient>>();

                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                        .WaitAndRetryAsync(
                            options.MaxRetryAttempts,
                            retryAttempt => options.UseExponentialBackoff
                                ? TimeSpan.FromSeconds(Math.Pow(options.BaseDelaySeconds, retryAttempt))
                                : TimeSpan.FromSeconds(options.BaseDelaySeconds),
                            (_, timespan, retryCount, _) =>
                            {
                                logger.LogWarning(
                                    "Retry {RetryCount} after {Timespan}s for {Method} {Uri}",
                                    retryCount,
                                    timespan.TotalSeconds,
                                    request.Method,
                                    request.RequestUri);
                            });
                })
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        5,
                        TimeSpan.FromSeconds(30)));

            // Register delegating handlers
            services.AddTransient<ApiKeyAuthenticationHandler>();
            services.AddTransient<IdempotencyHandler>();
            services.AddTransient<RateLimitHandler>();

            // Add webhook services
            services.AddKulipaWebhookServices();

            // Add other resources as they're implemented
            // services.AddScoped<IUsersResource, UsersResource>();
            // services.AddScoped<ITransactionsResource, TransactionsResource>();

            return services;
        }

        private static void ConfigureHttpClient(HttpClient client, KulipaSdkOptions options)
        {
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.Timeout;
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent",
                $"Kulipa-SDK-Dotnet/{typeof(KulipaClient).Assembly.GetName().Version}");
        }
    }
}