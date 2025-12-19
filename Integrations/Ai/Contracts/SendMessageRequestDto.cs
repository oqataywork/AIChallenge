namespace Integrations.Ai.Contracts;

public class SendMessageRequestDto
{
    public string Prompt { get; set; }
    public float Temperature { get; set; }

    public SendMessageRequestDto(string prompt)
    {
        Prompt = prompt;
    }
}