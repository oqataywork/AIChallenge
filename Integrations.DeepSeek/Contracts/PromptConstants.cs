namespace Integrations.DeepSeek.Contracts;

internal static class PromptConstants
{
    public const string ResponseFormat = "{\"ModelName\":\"\",\"Question\":\"\",\"Answer\":\"\"}";
    public const string Separator = "||| ";

    public const string PromptMessage = $""
        + $"Отвечай только в таком формате. "
        + $"Указанный формат:{ResponseFormat} "
        + $"Не добавляй лишних слов. Только JSON из указанного формата."
        + $"В ModelName укажи название LLM которая формирует ответ "
        + $"В Question укажи текст сообщения на который отвечаешь после символов {Separator} "
        + $"В Answer запиши ответ полученный из твоих вычислений "
        + Separator;
}