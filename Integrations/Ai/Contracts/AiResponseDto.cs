namespace Integrations.Ai.Contracts;

public class AiResponseDto
{
    public string Question { get; set; }

    //TODO: сделать отдельное поле для сжатого ответа
    public string Answer { get; set; }
    public string ResponseSource { get; set; }
    public bool IsFinished { get; set; }
}