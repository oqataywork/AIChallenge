using Domain;

namespace Infrastructure.Context;

public class AiContextDb
{
    public string Question { get; set; }
    public string Answer { get; set; }

    public AiContextDb(string question, string answer)
    {
        Question = question;
        Answer = answer;
    }

    public AiContext ToDomain()
    {
        return new AiContext(Question, Answer);
    }

    public static AiContextDb FromDomain(string question, string answer)
    {
        return new AiContextDb(question, answer);
    }

    public static AiContextDb FromDomain(AiContext aiContext)
    {
        return new AiContextDb(aiContext.Question, aiContext.Answer);
    }
}