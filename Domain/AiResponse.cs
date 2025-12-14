namespace Domain;

public class AiResponse
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public bool IsFinished { get; set; }

    public AiResponse(
        string question,
        string answer,
        bool isFinished)
    {
        Question = question;
        Answer = answer;
        IsFinished = isFinished;
    }
}