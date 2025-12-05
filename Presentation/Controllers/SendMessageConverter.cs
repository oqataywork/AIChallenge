using DomainService.Contracts;

using Presentation.Controllers.Contracts;

namespace Presentation.Controllers;

public static class SendMessageConverter
{
    public static SendMessageRequestInternal Convert(SendMessageRequest message)
    {
        return new SendMessageRequestInternal(message.UserMessage, Convert(message.SystemPromptType));
    }

    private static SystemPromptType Convert(string messageSystemPromptType)
    {
        return messageSystemPromptType switch
        {
            "Base" => SystemPromptType.Base,
            "Analytical" => SystemPromptType.Analytical,
            "WithoutContext" => SystemPromptType.WithoutContext,
            _ => throw new ArgumentException($"Unknown prompt type: {messageSystemPromptType}")
        };
    }
}