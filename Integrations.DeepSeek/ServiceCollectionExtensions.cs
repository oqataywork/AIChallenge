using Microsoft.Extensions.DependencyInjection;

namespace Integrations.DeepSeek;

public static class ServiceCollectionExtensions
{
    public static void AddDeepSeek(this IServiceCollection services)
    {
        services.AddOptions<DeepSeekOptions>()
            .Configure(
                options =>
                {
                    options.ApiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")
                        ?? throw new InvalidOperationException("DEEPSEEK_API_KEY is not set");
                });

        services.AddSingleton<IDeepSeekAiClient, DeepSeekAiClient>();
    }
}