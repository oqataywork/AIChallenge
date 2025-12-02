using Microsoft.Extensions.DependencyInjection;

namespace Integrations.DeepSeek;

public static class ServiceCollectionExtensions
{
    public static void AddDeepSeek(this IServiceCollection services)
    {
        string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");

        services.AddSingleton(new DeepSeekAiClient(apiKey));
    }
}