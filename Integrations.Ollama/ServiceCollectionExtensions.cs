using Integrations.Embedding;

using Microsoft.Extensions.DependencyInjection;

namespace Integrations.Ollama;

public static class ServiceCollectionExtensions
{
    public static void AddOllama(this IServiceCollection services)
    {
        services.AddSingleton<IOllamaClient, OllamaClient>();
    }
}