using DeepSeek;
using DeepSeek.Classes;

namespace Integrations.DeepSeek;

public class DeepSeekAiClient
{
    private readonly DeepSeekClient _client;

    public DeepSeekAiClient(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);
    }

    public DeepSeekAiClient(HttpClient httpClient, string apiKey)
    {
        _client = new DeepSeekClient(httpClient, apiKey);
    }

    public async Task<string> Send(string userMessage)
    {
        var request = new ChatRequest
        {
            Model = Models.ModelChat,
            Messages =
            [
                Message.NewUserMessage(userMessage)
            ]
        };

        ChatResponse? response = await _client.ChatAsync(request);

        if (response?.Choices == null || response.Choices.Count == 0)
        {
            return "No response from model";
        }

        return response.Choices.First().Message.Content;
    }
}