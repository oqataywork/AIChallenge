using System.Data;

using Infrastructure.Context;

using Microsoft.Extensions.DependencyInjection;

using Repository;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddRepository(this IServiceCollection services)
    {
        services.AddSingleton<IContextRepository, ContextRepository>();
    }
}