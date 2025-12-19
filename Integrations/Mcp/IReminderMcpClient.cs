using Integrations.Mcp.Contracts;

namespace Integrations.Mcp;

public interface IReminderMcpClient
{
    Task<string> AddReminder(
        AddReminderRequestDto request,
        CancellationToken cancellationToken);

    Task<string> GetReminders(GetRemindersRequestDto request, CancellationToken cancellationToken);
}