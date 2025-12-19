using Integrations.Mcp;
using Integrations.Mcp.Contracts;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Integrations.OgMcpClient;

public class ForecastMcpClient : McpServerClient, IForecastMcpClient
{
    public ForecastMcpClient(McpClient mcpClient) : base(mcpClient)
    {
        SupportedToolsRegistry.RegisterTool(["get_forecast"]);
    }

    public async Task<string> GetForecast(GetForecastRequestDto request, CancellationToken cancellationToken = default)
    {
        IList<McpClientTool> tools = await GetAvailableTools(cancellationToken);
        McpClientTool? forecastTool = tools.FirstOrDefault(t => t.Name == "get_forecast");

        if (forecastTool == null)
        {
            throw new InvalidOperationException("Tool 'GetForecast' not found on server.");
        }

        var args = new
        {
            latitude = request.Latitude,
            longitude = request.Longitude
        };

        IList<ContentBlock> result = await CallTool(forecastTool.Name, args, cancellationToken);

        var textBlock = result.First() as TextContentBlock;

        return textBlock?.Text ?? throw new Exception($"Failed to call forecast '{forecastTool.Name}'");
    }
}