namespace Integrations.DeepSeek.Contracts;

public class AiResponse
{
    public string ModelName { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
    public AiContext Context { get; set; }
}