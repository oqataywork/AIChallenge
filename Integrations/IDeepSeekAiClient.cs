using Integrations.Contracts;

namespace Integrations;

public interface IDeepSeekAiClient
{
    Task<AiResponseDto> SendToChat(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
    Task<AiResponseDto> SendToReasoner(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
}