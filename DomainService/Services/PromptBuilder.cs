using System.Text;

using Domain;

using DomainService.Contracts;

namespace DomainService.Services;

internal sealed class PromptBuilder : IPromptBuilder
{
    private string _userPrompt = string.Empty;
    private List<AiContext> _context = [];
    private bool _useContext = true;
    private double _temperature = DefaultTemperature;
    private int _compressionPercentage = DefaultCompressionPercentage;

    public IPromptBuilder WithUserPrompt(string prompt)
    {
        _userPrompt = prompt;

        return this;
    }

    public IPromptBuilder WithTemperature(double temperature)
    {
        _temperature = temperature;

        return this;
    }

    public IPromptBuilder WithContext(List<AiContext> context)
    {
        _context = context;
        _useContext = true;

        return this;
    }

    public IPromptBuilder WithoutContext()
    {
        _useContext = false;

        return this;
    }

    public IPromptBuilder WithCompressionPercentage(int compressionPercentage)
    {
        _compressionPercentage = compressionPercentage;

        return this;
    }

    public string Build()
    {
        var sb = new StringBuilder();

        sb.Append(_userPrompt);

        sb.Append(ResponseFormatPrompt);

        if (_useContext)
        {
            sb.Append(CreateContextPrompt(_context));
        }

        sb.Append(FinishChatPrompt);
        sb.Append(CreateTemperaturePrompt());

        if (_useContext)
        {
            sb.Append(CreateCompressionPercentagePrompt());
        }

        return sb.ToString();
    }

    // ===== Private helpers & constants =====

    private const string ResponseFormatPrompt = $"{Separator}"
        + "Отвечай только в таком формате. "
        + $"Указанный формат:{ResponseFormat} "
        + "Не добавляй лишних слов. Только JSON из указанного формата."
        + "В Question укажи текст сообщения на который отвечаешь. "
        + $"Текст сообщения в запросе находится перед символами {Separator} "
        + "В Answer запиши ответ полученный из твоих вычислений. ";

    private const string FinishChatPrompt =
        "Если задача требует нескольких вопросов и ответов, " +
        "и ты пришёл к выводу, что она полностью решена, " +
        "укажи в поле IsFinished значение true. " +
        "Во всех остальных случаях указывай false.";

    private const string ResponseFormat = "{"
        + "\"Question\":\"string\","
        + "\"Answer\":\"string\","
        + "\"IsFinished\": bool"
        + "}";

    private const string Separator = "||| ";

    private const double DefaultTemperature = 0.7;
    private const int DefaultCompressionPercentage = 25;

    private static string CreateContextPrompt(List<AiContext> contexts)
    {
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

    private string CreateTemperaturePrompt()
    {
        return "Prompt Temperature is: " + _temperature;
    }

    //TODO: пофиксить
    private string CreateCompressionPercentagePrompt()
    {
        return $"Текст Question и Answer уменьши на {_compressionPercentage}%";
    }
}