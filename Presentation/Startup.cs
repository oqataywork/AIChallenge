using System.Data;

using DomainService;

using Infrastructure;

using Integrations.DeepSeek;
using Integrations.OpenAI;

using Npgsql;

namespace Presentation;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddHandlers();

        AddPostgres(services);

        AddExternalClients(services);
    }

    private void AddPostgres(IServiceCollection services)
    {
        services.AddScoped<IDbConnection>(
            _ =>
            {
                string? connectionString = Configuration.GetConnectionString("Postgres");

                return new NpgsqlConnection(connectionString);
            });

        services.AddRepository();
    }

    private static void AddExternalClients(IServiceCollection services)
    {
        services.AddDeepSeek();
        services.AddOpenAi();
    }

    public void Configure(WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
    }
}