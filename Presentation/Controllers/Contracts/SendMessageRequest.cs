namespace Presentation.Controllers.Contracts;

public class SendMessageRequest
{
    public string UserMessage { get; set; }
    public string SystemPromptType { get; set; }
}