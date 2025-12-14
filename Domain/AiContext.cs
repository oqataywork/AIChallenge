namespace Domain;

public class AiContext
{
    public string Question { get; set; }
    public string Answer { get; set; }

    public AiContext(string question, string answer)
    {
        Question = question;
        Answer = answer;
    }
}