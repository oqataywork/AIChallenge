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

    public async Task<AiResponse?> Send(string userMessage)
    {
        string prompt = Prompts.ResponseFormatPrompt + userMessage + Prompts.CreateContextPrompt(_context);

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
}