using Integrations.Mcp;

using Microsoft.Extensions.DependencyInjection;

using ModelContextProtocol.Client;

namespace Integrations.OgMcpClient;

public static class ServiceCollectionExtensions
{
    public static void AddOgMcpClient(this IServiceCollection services)
    {
        // Регистрируем MCP клиент как Singleton
        services.AddSingleton<McpClient>(
            _ =>
            {
                var clientTransport = new StdioClientTransport(
                    new StdioClientTransportOptions
                    {
                        Name = "Demo Server",
                        Command = "dotnet",
                        Arguments =
                        [
                            "run",
                            "--project",
                            @"C:\Users\ogtay\RiderProjects\McpServer\McpServerConsole\McpServerConsole.csproj",
                            "--no-build"
                        ]
                    });

                // Ждём синхронно создание MCP клиента
                return McpClient.CreateAsync(clientTransport).GetAwaiter().GetResult();
            });

        // Регистрируем свою обёртку
        services.AddScoped<IForecastMcpClient, ForecastMcpClient>();
        services.AddScoped<IReminderMcpClient, ReminderMcpClient>();
        services.AddScoped<IFileResearcherMcpClient, FileResearcherMcpClient>();
    }
}