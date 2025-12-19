namespace Integrations.Mcp.Contracts;

public class CreateFileRequestDto
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string Content { get; set; }
}