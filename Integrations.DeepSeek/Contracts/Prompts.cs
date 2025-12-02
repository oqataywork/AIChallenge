using System.Text;

namespace Integrations.DeepSeek.Contracts;

internal static class Prompts
{
    public const string ResponseFormat = "{\"ModelName\":\"\",\"Question\":\"\",\"Answer\":\"\",\"Context\":{\"Question\":\"\",\"Answer\":\"\"}}";
    public const string Separator = "||| ";

    public const string ResponseFormatPrompt = $"{Separator}"
        + $"Отвечай только в таком формате. "
        + $"Указанный формат:{ResponseFormat} "
        + $"Не добавляй лишних слов. Только JSON из указанного формата."
        + $"В ModelName укажи название LLM которая формирует ответ "
        + $"В Question укажи текст сообщения на который отвечаешь. "
        + $" Текст сообщения в запросе находится перед символами {Separator} "
        + $"В Answer запиши ответ полученный из твоих вычислений "
        + $"В Context запиши Question и Answer, но в сжатом формате (25%) ";

    public static string CreateContextPrompt(List<AiContext> contexts)
    {
        if (contexts.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        sb.AppendLine("###DIALOGUE_CONTEXT_START###");

        foreach (AiContext ctx in contexts)
        {
            string q = ctx.Question.Trim();
            string a = ctx.Answer.Trim();

            if (!string.IsNullOrWhiteSpace(q))
            {
                sb.AppendLine($"Q: {q}");
            }

            if (!string.IsNullOrWhiteSpace(a))
            {
                sb.AppendLine($"A: {a}");
            }

            sb.AppendLine("---");
        }

        sb.AppendLine("###DIALOGUE_CONTEXT_END###");

        return sb.ToString();
    }
}