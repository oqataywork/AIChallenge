using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Integrations.OgMcpClient;

public class OgMcpClient : IOgMcpClient
{
    private readonly McpClient _mcpClient;

    public OgMcpClient(McpClient mcpClient)
    {
        _mcpClient = mcpClient ?? throw new ArgumentNullException(nameof(mcpClient));
    }

    public async Task<IList<McpClientTool>> GetAvailableTools(CancellationToken cancellationToken = default)
    {
        return await _mcpClient.ListToolsAsync(cancellationToken: cancellationToken);
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

    public async Task<string> CallForecast(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        IList<McpClientTool> tools = await GetAvailableTools(cancellationToken);
        McpClientTool? forecastTool = tools.FirstOrDefault(t => t.Name == "get_forecast");

        if (forecastTool == null)
        {
            throw new InvalidOperationException("Tool 'GetForecast' not found on server.");
        }

        var args = new
        {
            latitude,
            longitude
        };

        IList<ContentBlock> result = await CallTool(forecastTool.Name, args, cancellationToken);

        var textBlock = result.First() as TextContentBlock;

        return textBlock?.Text ?? throw new Exception($"Failed to call forecast '{forecastTool.Name}'");
    }
}