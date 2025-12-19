using Integrations.Mcp;
using Integrations.Mcp.Contracts;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Integrations.OgMcpClient;

public class FileResearcherMcpClient : McpServerClient, IFileResearcherMcpClient
{
    public FileResearcherMcpClient(McpClient mcpClient) : base(mcpClient)
    {
        SupportedToolsRegistry.RegisterTool(["read_file", "create_file"]);
    }

    public async Task<string> ReadFile(ReadFileRequestDto request, CancellationToken cancellationToken)
    {
        IList<McpClientTool> tools = await GetAvailableTools(cancellationToken);
        McpClientTool? tool = tools.FirstOrDefault(t => t.Name == "read_file");

        if (tool == null)
        {
            throw new InvalidOperationException("Tool 'read_file' not found on server.");
        }

        var args = new
        {
            filePath = request.FilePath
        };

        IList<ContentBlock> result = await CallTool(tool.Name, args, cancellationToken);

        var textBlock = result.First() as TextContentBlock;

        return textBlock?.Text ?? throw new Exception($"Failed to call read_file '{tool.Name}'");
    }

    public async Task<string> CreateFile(CreateFileRequestDto request, CancellationToken cancellationToken)
    {
        IList<McpClientTool> tools = await GetAvailableTools(cancellationToken);
        McpClientTool? tool = tools.FirstOrDefault(t => t.Name == "create_file");

        if (tool == null)
        {
            throw new InvalidOperationException("Tool 'create_file' not found on server.");
        }

        var args = new
        {
            filePath = request.FilePath,
            fileName = request.FileName,
            content = request.Content,
        };

        IList<ContentBlock> result = await CallTool(tool.Name, args, cancellationToken);

        var textBlock = result.First() as TextContentBlock;

        return textBlock?.Text ?? throw new Exception($"Failed to call create_file '{tool.Name}'");
    }
}