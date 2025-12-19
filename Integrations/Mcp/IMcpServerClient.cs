using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Integrations.Mcp;

public interface IMcpServerClient
{
    Task<IList<McpClientTool>> GetAvailableTools(CancellationToken cancellationToken = default);
    Task<IList<ContentBlock>> CallTool(string toolName, object? args = null, CancellationToken cancellationToken = default);
}