using DomainService.Contracts;
using DomainService.Services;

using Microsoft.Extensions.DependencyInjection;

namespace DomainService;

public static class ServiceCollectionExtensions
{
    public static void AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<SendMessageHandler>();
        services.AddScoped<MessageSender>();
        services.AddScoped<IPromptBuilder, PromptBuilder>();
        services.AddScoped<IRagContextService, RagContextService>();

        // services.AddHostedService<RemindersSummaryHostedService>();
    }
}