namespace Domain;

public class AiContext
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public ResponseSource ResponseSource { get; set; }

    public AiContext(string question, string answer, ResponseSource responseSource)
    {
        Question = question;
        Answer = answer;
        ResponseSource = responseSource;
    }
}