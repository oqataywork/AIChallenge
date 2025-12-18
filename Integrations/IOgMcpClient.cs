using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Integrations;

public interface IOgMcpClient
{
    Task<IList<McpClientTool>> GetAvailableTools(CancellationToken cancellationToken = default);
    Task<IList<ContentBlock>> CallTool(string toolName, object? args = null, CancellationToken cancellationToken = default);

    Task<string> CallForecast(double latitude, double longitude, CancellationToken cancellationToken = default);
}