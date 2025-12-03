namespace Integrations.DeepSeek.Contracts;

public class AiResponse
{
    public string Question { get; set; }
    public string Answer { get; set; }

    public bool IsFinished { get; set; }

    public AiContext Context { get; set; }
}