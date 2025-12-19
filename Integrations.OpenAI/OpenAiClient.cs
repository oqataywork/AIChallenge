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
    private readonly IFileResearcherMcpClient _fileResearcherMcpClient;

    public OpenAiClient(
        IOptions<OpenAiOptions> options,
        IForecastMcpClient forecastMcpClient,
        IReminderMcpClient reminderMcpClient,
        IFileResearcherMcpClient fileResearcherMcpClient)
    {
        _forecastMcpClient = forecastMcpClient;
        _reminderMcpClient = reminderMcpClient;
        _fileResearcherMcpClient = fileResearcherMcpClient;

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

        ClientResult<ChatCompletion> openAiResponse =
            await chatClient.CompleteChatAsync(messages, options, cancellationToken: cancellationToken);

        while (openAiResponse.Value.FinishReason == ChatFinishReason.ToolCalls)
        {
            messages.Add(ChatMessage.CreateAssistantMessage(openAiResponse.Value.ToolCalls));

            foreach (ChatToolCall call in openAiResponse.Value.ToolCalls)
            {
                string result = await SelectRequiredMethod(call, cancellationToken);

                messages.Add(
                    ChatMessage.CreateToolMessage(
                        toolCallId: call.Id,
                        content: result));
            }

            openAiResponse = await chatClient.CompleteChatAsync(
                messages,
                options,
                cancellationToken: cancellationToken);
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
            "read_file" => await _fileResearcherMcpClient.ReadFile(
                JsonSerializer.Deserialize<ReadFileRequestDto>(call.FunctionArguments, serializerOptions),
                cancellationToken),
            "create_file" => await _fileResearcherMcpClient.CreateFile(
                JsonSerializer.Deserialize<CreateFileRequestDto>(call.FunctionArguments, serializerOptions),
                cancellationToken),
        };
    }

    private static ChatCompletionOptions ConfigureOptions(IList<McpClientTool> tools)
    {
        ChatTool functionTool0 = GetToolByIndex(tools, 0);
        ChatTool functionTool1 = GetToolByIndex(tools, 1);
        ChatTool functionTool2 = GetToolByIndex(tools, 2);
        ChatTool functionTool3 = GetToolByIndex(tools, 3);
        ChatTool functionTool4 = GetToolByIndex(tools, 4);

        return new ChatCompletionOptions()
        {
            // Temperature = ,
            // ResponseFormat = ,
            ToolChoice = ChatToolChoice.CreateAutoChoice(),
            Tools = { functionTool0, functionTool1, functionTool2, functionTool3, functionTool4 }
        };
    }

    private static ChatTool GetToolByIndex(IList<McpClientTool> tools, int index)
    {
        return ChatTool.CreateFunctionTool(
            functionName: tools[index].Name,
            functionDescription: tools[index].Description,
            functionParameters: BinaryData.FromString($"""{tools[index].ProtocolTool.InputSchema.ToString()}"""));
    }
}