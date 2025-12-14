namespace Presentation.Controllers.Contracts;

public class SendMessageResponse
{
    public string Answer { get; set; }
    public bool IsFinished { get; set; }

    public SendMessageResponse(string answer, bool isFinished)
    {
        Answer = answer;
        IsFinished = isFinished;
    }
}