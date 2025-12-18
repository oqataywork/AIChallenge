using Integrations.Contracts;

namespace Integrations;

public interface IOpenAiClient
{
    Task<AiResponseDto> SendToGpt5Nano(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
    Task<AiResponseDto> SendToGpt5Dot1(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
}