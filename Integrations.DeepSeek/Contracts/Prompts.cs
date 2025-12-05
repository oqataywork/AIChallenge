using System.Text;
using System.Text.Json;

namespace Integrations.DeepSeek.Contracts;

internal static class Prompts
{
    public static string CreateBasePrompt(string userPrompt, List<AiContext> context)
    {
        return ResponseFormatPrompt + userPrompt + CreateContextPrompt(context) + FinishChatPrompt;
    }

    public static string CreateAnalyticalPrompt(string userPrompt, List<AiContext> context)
    {
        return $"{ResponseFormatPrompt} "
            + userPrompt
            + AnalyticalModifier
            + CreateContextPrompt(context)
            + FinishChatPrompt;
    }

    public static string CreateWithoutContextPrompt(string userPrompt)
    {
        return ResponseFormatPrompt + userPrompt + FinishChatPrompt;
    }

    private const string AnalyticalModifier =
        "Используй аналитический режим. Давай более структурированный, логичный ответ, " +
        "делай небольшие выводы, но всё равно заполняй только JSON из указанного формата. " +
        "Не выходи за рамки ResponseFormat.";

    private const string ResponseFormat = ""
        + "\"Question\":\"\","
        + "\"Answer\":\"\","
        + "\"IsFinished\":,"
        + "\"Context\":{\"Question\":\"\",\"Answer\":\"\"}}";

    private const string Separator = "||| ";

    private const string ResponseFormatPrompt = $"{Separator}"
        + $"Отвечай только в таком формате. "
        + $"Указанный формат:{ResponseFormat} "
        + $"Не добавляй лишних слов. Только JSON из указанного формата."
        + $"В Question укажи текст сообщения на который отвечаешь. "
        + $" Текст сообщения в запросе находится перед символами {Separator} "
        + $"В Answer запиши ответ полученный из твоих вычислений "
        + $"В Context запиши Question и Answer, но в сжатом формате (25%) ";

    private const string FinishChatPrompt = $"Если задача сложная, требующая нескольких вопросов,"
        + $"а ты понял, что задача завершена, то"
        + $"в IsFinished (bool) положи true,"
        + $" если посчитаешь, что задача выполнена. Во всех остальные случаях false";

    private static string CreateContextPrompt(List<AiContext> contexts)
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