namespace Domain;

public class AiResponse
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public ResponseSource ResponseSource { get; set; }
    public bool IsFinished { get; set; }

    public AiResponse(
        string question,
        string answer,
        bool isFinished,
        ResponseSource responseSource)
    {
        Question = question;
        Answer = answer;
        IsFinished = isFinished;
        ResponseSource = responseSource;
    }
}