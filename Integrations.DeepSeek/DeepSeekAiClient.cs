using System.Text.Json;

using DeepSeek;
using DeepSeek.Classes;

using Integrations.Contracts;

namespace Integrations.DeepSeek;

using Microsoft.Extensions.Options;

public class DeepSeekAiClient : IDeepSeekAiClient
{
    private readonly DeepSeekClient _client;

    public DeepSeekAiClient(IOptions<DeepSeekOptions> options)
    {
        string apiKey = options.Value.ApiKey;
        _client = new DeepSeekClient(apiKey);
    }

    public async Task<AiResponseDto?> SendToChat(SendMessageRequestDto requestDto, CancellationToken cancellationToken)
    {
        var request = new ChatRequest
        {
            Model = Models.ModelChat,
            Messages =
            [
                Message.NewUserMessage(requestDto.Prompt)
            ]
        };

        ChatResponse? response = await _client.ChatAsync(request, cancellationToken);

        if (response?.Choices == null || response.Choices.Count == 0)
        {
            return null;
        }

        var aiResponse = JsonSerializer.Deserialize<AiResponseDto>(response.Choices.First().Message.Content);

        return aiResponse;
    }

    public async Task<AiResponseDto?> SendToReasoner(SendMessageRequestDto requestDto, CancellationToken cancellationToken)
    {
        var request = new ChatRequest
        {
            Model = Models.ModelReasoner,
            Messages =
            [
                Message.NewUserMessage(requestDto.Prompt)
            ]
        };

        ChatResponse? response = await _client.ChatAsync(request, cancellationToken);

        if (response?.Choices == null || response.Choices.Count == 0)
        {
            return null;
        }

        var aiResponse = JsonSerializer.Deserialize<AiResponseDto>(response.Choices.First().Message.Content);

        return aiResponse;
    }
}