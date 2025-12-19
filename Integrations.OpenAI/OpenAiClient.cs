using System.ClientModel;
using System.Text.Json;

using Integrations.Ai;
using Integrations.Ai.Contracts;
using Integrations.Mcp;
using Integrations.Mcp.Contracts;

using Microsoft.Extensions.Options;

using ModelContextProtocol.Client;

using OpenAI;
using OpenAI.Chat;

namespace Integrations.OpenAI;

public class OpenAiClient : IOpenAiClient
{
    private readonly OpenAIClient _client;
    private readonly IForecastMcpClient _forecastMcpClient;
    private readonly IReminderMcpClient _reminderMcpClient;

    public OpenAiClient(IOptions<OpenAiOptions> options, IForecastMcpClient forecastMcpClient, IReminderMcpClient reminderMcpClient)
    {
        _forecastMcpClient = forecastMcpClient;
        _reminderMcpClient = reminderMcpClient;

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

    public async Task<AiResponseDto> SendToGpt5Dot1(SendMessageRequestDto requestDto, CancellationToken cancellationToken)
    {
        ChatClient chatClient = _client.GetChatClient("gpt-5.1");

        //TODO: исправить что именно наследник дает дженерик результат
        IList<McpClientTool> tools = await _forecastMcpClient.GetAvailableTools(cancellationToken);

        ChatCompletionOptions options = ConfigureOptions(tools);

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateUserMessage(requestDto.Prompt)
        };

        ClientResult<ChatCompletion> openAiResponse = await chatClient.CompleteChatAsync(messages, options, cancellationToken: cancellationToken);

        if (openAiResponse.Value.FinishReason is ChatFinishReason.ToolCalls)
        {
            messages.Add(ChatMessage.CreateAssistantMessage(openAiResponse.Value.ToolCalls));

            foreach (ChatToolCall call in openAiResponse.Value.ToolCalls)
            {
                string result = await SelectRequiredMethod(call, cancellationToken);
                messages.Add(ChatMessage.CreateToolMessage(call.Id, result));
            }

            openAiResponse = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        }

        var aiResponse = JsonSerializer.Deserialize<AiResponseDto>(openAiResponse.Value.Content.First().Text);

        return aiResponse;
    }

    private async Task<string> SelectRequiredMethod(ChatToolCall call, CancellationToken cancellationToken)
    {
        var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        return call.FunctionName switch
        {
            "get_forecast" => await _forecastMcpClient.GetForecast(
                JsonSerializer.Deserialize<GetForecastRequestDto>(call.FunctionArguments, serializerOptions),
                cancellationToken),
            "get_reminders" => await _reminderMcpClient.GetReminders(
                JsonSerializer.Deserialize<GetRemindersRequestDto>(call.FunctionArguments, serializerOptions),
                cancellationToken),
            "add_reminder" => await _reminderMcpClient.AddReminder(
                JsonSerializer.Deserialize<AddReminderRequestDto>(call.FunctionArguments, serializerOptions),
                cancellationToken),
        };
    }

    private static ChatCompletionOptions ConfigureOptions(IList<McpClientTool> tools)
    {
        var functionTool0 = ChatTool.CreateFunctionTool(
            functionName: tools[0].Name,
            functionDescription: tools[0].Description,
            functionParameters: BinaryData.FromString($"""{tools[0].ProtocolTool.InputSchema.ToString()}"""));

        var functionTool1 = ChatTool.CreateFunctionTool(
            functionName: tools[1].Name,
            functionDescription: tools[1].Description,
            functionParameters: BinaryData.FromString($"""{tools[1].ProtocolTool.InputSchema.ToString()}"""));

        var functionTool2 = ChatTool.CreateFunctionTool(
            functionName: tools[2].Name,
            functionDescription: tools[2].Description,
            functionParameters: BinaryData.FromString($"""{tools[2].ProtocolTool.InputSchema.ToString()}"""));

        return new ChatCompletionOptions()
        {
            // Temperature = ,
            // ResponseFormat = ,
            ToolChoice = ChatToolChoice.CreateRequiredChoice(),
            Tools = { functionTool0, functionTool1, functionTool2 }
        };
    }
}