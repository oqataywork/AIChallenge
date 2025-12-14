using Domain;

using Integrations;
using Integrations.Contracts;

namespace DomainService.Services;

public class MessageSender
{
    private readonly IDeepSeekAiClient _deepSeekAiClient;
    private readonly IOpenAiClient _openAiClient;

    public MessageSender(IDeepSeekAiClient deepSeekAiClient, IOpenAiClient openAiClient)
    {
        _deepSeekAiClient = deepSeekAiClient;
        _openAiClient = openAiClient;
    }

    public async Task<AiResponse> SendMessage(string prompt, ModelType modelType, CancellationToken cancellationToken)
    {
        SendMessageRequestDto requestDto = SendMessageConverter.Convert(prompt);

        AiResponseDto responseDto = modelType switch
        {
            ModelType.DeepSeekChat => await _deepSeekAiClient.SendToChat(requestDto, cancellationToken),
            ModelType.DeepSeekReasoner => await _deepSeekAiClient.SendToReasoner(requestDto, cancellationToken),
            ModelType.OpenAiGpt5Nano => await _openAiClient.SendToGpt5Nano(requestDto, cancellationToken),
            ModelType.OpenAiGpt5Dot1 => await _openAiClient.Gpt5Dot1(requestDto, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(modelType), modelType, null)
        };

        return SendMessageConverter.Convert(responseDto);
    }
}