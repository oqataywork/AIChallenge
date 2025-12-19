using Integrations.Ai.Contracts;

namespace Integrations.Ai;

public interface IDeepSeekAiClient
{
    Task<AiResponseDto> SendToChat(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
    Task<AiResponseDto> SendToReasoner(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
}