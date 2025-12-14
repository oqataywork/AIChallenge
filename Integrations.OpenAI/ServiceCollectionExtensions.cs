using Microsoft.Extensions.DependencyInjection;

namespace Integrations.OpenAI;

public static class ServiceCollectionExtensions
{
    public static void AddOpenAi(this IServiceCollection services)
    {
        services.AddOptions<OpenAiOptions>()
            .Configure(
                options =>
                {
                    options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                        ?? throw new InvalidOperationException("OPENAI_API_KEY is not set");
                });

        services.AddSingleton<IOpenAiClient, OpenAiClient>();
    }
}