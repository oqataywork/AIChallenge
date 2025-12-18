using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Nodes;

using Integrations.Contracts;

using Microsoft.Extensions.Options;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

using OpenAI;
using OpenAI.Chat;

namespace Integrations.OpenAI;

public class OpenAiClient : IOpenAiClient
{
    private readonly OpenAIClient _client;
    private readonly IOgMcpClient _mcpClient;

    public OpenAiClient(IOptions<OpenAiOptions> options, IOgMcpClient mcpClient)
    {
        _mcpClient = mcpClient;
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

        IList<McpClientTool> tools = await _mcpClient.GetAvailableTools(cancellationToken);
        McpClientTool tool = tools.First();

        ChatCompletionOptions options = ConfigureOptions(tool);

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
        return call.FunctionName switch
        {
            "get_forecast" => await _mcpClient.CallForecast(55.75, 37.62, cancellationToken),
        };
    }

    private static ChatCompletionOptions ConfigureOptions(McpClientTool tool)
    {
        var functionTool = ChatTool.CreateFunctionTool(
            functionName: tool.Name,
            functionDescription: tool.Description,
            functionParameters: BinaryData.FromString($"""{tool.ProtocolTool.InputSchema.ToString()}"""));

        return new ChatCompletionOptions()
        {
            //TODO: прокинуть поляхи
            //Temperature = requestDto.Temperature,
            //ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(BinaryData.FromBytes()),
            ToolChoice = ChatToolChoice.CreateAutoChoice(),
            Tools = { functionTool },
        };
    }
}