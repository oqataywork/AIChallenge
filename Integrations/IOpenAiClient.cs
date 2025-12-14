using Integrations.Contracts;

namespace Integrations;

public interface IOpenAiClient
{
    Task<AiResponseDto> SendToGpt5Nano(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
    Task<AiResponseDto> Gpt5Dot1(SendMessageRequestDto requestDto, CancellationToken cancellationToken);
}