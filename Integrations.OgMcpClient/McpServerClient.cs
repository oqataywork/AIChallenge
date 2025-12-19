using Integrations.Mcp;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Integrations.OgMcpClient;

public class McpServerClient : IMcpServerClient
{
    private readonly McpClient _mcpClient;

    public McpServerClient(McpClient mcpClient)
    {
        _mcpClient = mcpClient ?? throw new ArgumentNullException(nameof(mcpClient));
    }

    public async Task<IList<McpClientTool>> GetAvailableTools(CancellationToken cancellationToken = default)
    {
        IList<McpClientTool> allTools = await _mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

        List<McpClientTool> filteredTools = allTools.Where(t => SupportedToolsRegistry.Tools.Contains(t.Name)).ToList();

        return filteredTools;
    }

    public async Task<IList<ContentBlock>> CallTool(string toolName, object? args = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(toolName))
        {
            throw new ArgumentException("Tool name must be provided", nameof(toolName));
        }

        IReadOnlyDictionary<string, object?>? arguments = args?.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(args));

        try
        {
            CallToolResult result = await _mcpClient.CallToolAsync(toolName, arguments, cancellationToken: cancellationToken);

            return result.Content;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to call tool '{toolName}'", ex);
        }
    }
}