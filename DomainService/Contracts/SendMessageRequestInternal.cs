namespace DomainService.Contracts;

public record SendMessageRequestInternal(string UserMessage, SystemPromptType SystemPromptType);