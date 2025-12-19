using Integrations.Mcp.Contracts;

namespace Integrations.Mcp;

public interface IForecastMcpClient : IMcpServerClient
{
    Task<string> GetForecast(GetForecastRequestDto request, CancellationToken cancellationToken = default);
}