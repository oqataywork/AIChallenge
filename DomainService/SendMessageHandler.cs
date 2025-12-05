using DomainService.Contracts;

using Integrations.DeepSeek;
using Integrations.DeepSeek.Contracts;

namespace DomainService;

public class SendMessageHandler
{
    private readonly DeepSeekAiClient _deepSeekAiClient;

    public SendMessageHandler(DeepSeekAiClient deepSeekAiClient)
    {
        _deepSeekAiClient = deepSeekAiClient;
    }

    public async Task<AiResponse?> Handle(SendMessageRequestInternal message)
    {
        SendMessageRequestDto requestDto = SendMessageConverter.Convert(message);

        AiResponse? response = await _deepSeekAiClient.Send(requestDto);

        return response;
    }
}