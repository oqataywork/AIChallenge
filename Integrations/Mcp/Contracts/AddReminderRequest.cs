namespace Integrations.Mcp.Contracts;

public class AddReminderRequestDto
{
    public DateTime RemindAt { get; set; }
    public string Task { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionTime { get; set; }
}