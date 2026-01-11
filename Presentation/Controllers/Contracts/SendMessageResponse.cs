namespace Presentation.Controllers.Contracts;

public class SendMessageResponse
{
    public string Answer { get; set; }
    public bool IsFinished { get; set; }
    public string ResponseSource { get; set; }

    public SendMessageResponse(string answer, bool isFinished, string responseSource)
    {
        Answer = answer;
        IsFinished = isFinished;
        ResponseSource = responseSource;
    }
}