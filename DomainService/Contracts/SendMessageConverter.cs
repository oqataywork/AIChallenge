using Integrations.DeepSeek.Contracts;

namespace DomainService.Contracts;

public static class SendMessageConverter
{
    public static SendMessageRequestDto Convert(SendMessageRequestInternal message)
    {
        return new SendMessageRequestDto(message.UserMessage, Convert(message.SystemPromptType));
    }

    private static SystemPromptTypeDto Convert(SystemPromptType systemPromptType)
    {
        return systemPromptType switch
        {
            SystemPromptType.Base => SystemPromptTypeDto.Base,
            SystemPromptType.Analytical => SystemPromptTypeDto.Analytical,
            SystemPromptType.WithoutContext => SystemPromptTypeDto.WithoutContext,
            _ => throw new ArgumentException($"Unknown prompt type: {systemPromptType}")
        };
    }
}