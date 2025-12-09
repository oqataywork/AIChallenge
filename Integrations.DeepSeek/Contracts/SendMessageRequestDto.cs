namespace Integrations.DeepSeek.Contracts;

public record SendMessageRequestDto(string UserMessage, SystemPromptTypeDto SystemPromptTypeDto, double Temperature);