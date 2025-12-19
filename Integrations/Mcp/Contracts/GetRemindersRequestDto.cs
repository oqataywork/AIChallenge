namespace Integrations.Mcp.Contracts;

public class GetRemindersRequestDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}