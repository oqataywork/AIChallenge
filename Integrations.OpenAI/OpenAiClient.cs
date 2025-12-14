using System.ClientModel;
using System.Text.Json;

using Integrations.Contracts;

using Microsoft.Extensions.Options;

using OpenAI;
using OpenAI.Chat;

namespace Integrations.OpenAI;

public class OpenAiClient : IOpenAiClient
{
    private readonly OpenAIClient _client;

    public OpenAiClient(IOptions<OpenAiOptions> options)
    {
        string apiKey = options.Value.ApiKey;
        _client = new OpenAIClient(apiKey);
    }

    public async Task<AiResponseDto> SendToGpt5Nano(SendMessageRequestDto requestDto, CancellationToken cancellationToken)
    {
        ChatClient chatClient = _client.GetChatClient("gpt-5-nano");

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateUserMessage(requestDto.Prompt)
        };

        ClientResult<ChatCompletion> openAiResponse = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);

        var aiResponse = JsonSerializer.Deserialize<AiResponseDto>(openAiResponse.Value.Content.First().Text);

        return aiResponse;
    }

    public async Task<AiResponseDto> Gpt5Dot1(SendMessageRequestDto requestDto, CancellationToken cancellationToken)
    {
        ChatClient chatClient = _client.GetChatClient("gpt-5.1");

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateUserMessage(requestDto.Prompt)
        };

        ClientResult<ChatCompletion> openAiResponse = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);

        var aiResponse = JsonSerializer.Deserialize<AiResponseDto>(openAiResponse.Value.Content.First().Text);

        return aiResponse;
    }
}