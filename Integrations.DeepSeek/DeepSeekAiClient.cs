using System.Text.Json;

using DeepSeek;
using DeepSeek.Classes;

using Integrations.DeepSeek.Contracts;

namespace Integrations.DeepSeek;

public class DeepSeekAiClient
{
    private readonly DeepSeekClient _client;
    private readonly List<AiContext> _context = [];

    public DeepSeekAiClient(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);
    }

    public async Task<AiResponse?> Send(SendMessageRequestDto requestDto)
    {
        string prompt = GetRequestedPrompt(requestDto);

        prompt = prompt + "Prompt Temperature is: " + requestDto.Temperature;

        var request = new ChatRequest
        {
            Model = Models.ModelChat,
            Messages =
            [
                Message.NewUserMessage(prompt)
            ]
        };

        ChatResponse? response = await _client.ChatAsync(request);

        if (response?.Choices == null || response.Choices.Count == 0)
        {
            return null;
        }

        var aiResponse = JsonSerializer.Deserialize<AiResponse>(response.Choices.First().Message.Content);

        _context.Add(aiResponse.Context);

        return aiResponse;
    }

    private string GetRequestedPrompt(SendMessageRequestDto requestDto)
    {
        return requestDto.SystemPromptTypeDto switch
        {
            SystemPromptTypeDto.Base => Prompts.CreateBasePrompt(requestDto.UserMessage, _context),
            SystemPromptTypeDto.Analytical => Prompts.CreateAnalyticalPrompt(requestDto.UserMessage, _context),
            SystemPromptTypeDto.WithoutContext => Prompts.CreateWithoutContextPrompt(requestDto.UserMessage),
            _ => throw new ArgumentException($"Unknown prompt type: {requestDto.SystemPromptTypeDto}")
        };
    }
}