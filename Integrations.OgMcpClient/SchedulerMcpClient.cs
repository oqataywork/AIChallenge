using Integrations.Mcp;
using Integrations.Mcp.Contracts;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Integrations.OgMcpClient;

public class ReminderMcpClient : McpServerClient, IReminderMcpClient
{
    public ReminderMcpClient(McpClient mcpClient) : base(mcpClient)
    {
        SupportedToolsRegistry.RegisterTool(["add_reminder", ("get_reminders")]);
    }

    public async Task<string> AddReminder(AddReminderRequestDto request, CancellationToken cancellationToken)
    {
        IList<McpClientTool> tools = await GetAvailableTools(cancellationToken);
        McpClientTool? tool = tools.FirstOrDefault(t => t.Name == "add_reminder");

        if (tool == null)
        {
            throw new InvalidOperationException("Tool 'add_reminder' not found on server.");
        }

        var args = new
        {
            remindAt = request.RemindAt,
            task = request.Task,
            isCompleted = request.IsCompleted,
            completionTime = request.CompletionTime,
        };

        IList<ContentBlock> result = await CallTool(tool.Name, args, cancellationToken);

        var textBlock = result.First() as TextContentBlock;

        return textBlock?.Text ?? throw new Exception($"Failed to call add_reminder '{tool.Name}'");
    }

    public async Task<string> GetReminders(GetRemindersRequestDto request, CancellationToken cancellationToken)
    {
        IList<McpClientTool> tools = await GetAvailableTools(cancellationToken);
        McpClientTool? tool = tools.FirstOrDefault(t => t.Name == "get_reminders");

        if (tool == null)
        {
            throw new InvalidOperationException("Tool 'get_reminders' not found on server.");
        }

        var args = new
        {
            from = request.From,
            to = request.To
        };

        IList<ContentBlock> result = await CallTool(tool.Name, args, cancellationToken);

        var textBlock = result.First() as TextContentBlock;

        return textBlock?.Text ?? throw new Exception($"Failed to call get_reminders '{tool.Name}'");
    }
}