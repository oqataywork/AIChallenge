// See https://aka.ms/new-console-template for more information

using System.ClientModel;
using System.Diagnostics;

using DeepSeek;
using DeepSeek.Classes;

using Google.GenAI;
using Google.GenAI.Types;

using OpenAI;
using OpenAI.Chat;

using Environment = System.Environment;
using Models = DeepSeek.Models;

var taskPrompt = "Predict how the gaming market will change in 2025–2030.";

//await ProcessGeminiFlash2Lite();

await ProcessGemini3ProPreview();
await ProcessDeepSeekChat(taskPrompt);
await ProcessDeepSeekReasoner(taskPrompt);
await ProcessOpenAiGpt5Nano(taskPrompt);
await ProcessOpenAiGpt5Dot1(taskPrompt);

async Task ProcessGemini3ProPreview()
{
    var geminiClient = new Client();

    var swGemini = Stopwatch.StartNew();
    GenerateContentResponse geminiResponse = await geminiClient.Models.GenerateContentAsync(
        model: "gemini-3-pro-preview",
        contents: taskPrompt);
    swGemini.Stop();

    string geminiAnswer = geminiResponse.Candidates[0].Content.Parts[0].Text;
    long geminiInputTokens = geminiResponse.UsageMetadata?.PromptTokenCount ?? -1;
    long geminiOutputTokens = geminiResponse.UsageMetadata?.CandidatesTokenCount ?? -1;

    DumpResult("gemini-3-pro-preview RESULT", swGemini.ElapsedMilliseconds, geminiInputTokens, geminiOutputTokens, geminiAnswer);
}

async Task ProcessGeminiFlash2Lite()
{
    var geminiClient = new Client();

    var swGemini = Stopwatch.StartNew();
    GenerateContentResponse geminiResponse = await geminiClient.Models.GenerateContentAsync(
        model: "gemini-2.0-flash-lite",
        contents: taskPrompt);
    swGemini.Stop();

    string geminiAnswer = geminiResponse.Candidates[0].Content.Parts[0].Text;
    long geminiInputTokens = geminiResponse.UsageMetadata?.PromptTokenCount ?? -1;
    long geminiOutputTokens = geminiResponse.UsageMetadata?.CandidatesTokenCount ?? -1;

    DumpResult("gemini-2.0-flash-lite RESULT", swGemini.ElapsedMilliseconds, geminiInputTokens, geminiOutputTokens, geminiAnswer);
}

async Task ProcessDeepSeekChat(string s)
{
    var deepSeekClient = new DeepSeekClient(Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY"));

    var deepSeekRequest = new ChatRequest
    {
        Model = Models.ModelChat,
        Messages = [Message.NewUserMessage(s)]
    };

    var swDeep = Stopwatch.StartNew();
    ChatResponse? deepResponse = await deepSeekClient.ChatAsync(deepSeekRequest);
    swDeep.Stop();

    string deepText = deepResponse.Choices[0].Message.Content;
    long deepInTokens = deepResponse.Usage?.PromptTokens ?? -1;
    long deepOutTokens = deepResponse.Usage?.CompletionTokens ?? -1;

    DumpResult("DEEPSEEK CHAT RESULT", swDeep.ElapsedMilliseconds, deepInTokens, deepOutTokens, deepText);
}

async Task ProcessDeepSeekReasoner(string s)
{
    var deepSeekClient = new DeepSeekClient(Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY"));

    var deepSeekRequest = new ChatRequest
    {
        Model = Models.ModelReasoner,
        Messages = [Message.NewUserMessage(s)]
    };

    var swDeep = Stopwatch.StartNew();
    ChatResponse? deepResponse = await deepSeekClient.ChatAsync(deepSeekRequest);
    swDeep.Stop();

    string deepText = deepResponse.Choices[0].Message.Content;
    long deepInTokens = deepResponse.Usage?.PromptTokens ?? -1;
    long deepOutTokens = deepResponse.Usage?.CompletionTokens ?? -1;

    DumpResult("DEEPSEEK REASONER RESULT", swDeep.ElapsedMilliseconds, deepInTokens, deepOutTokens, deepText);
}

async Task ProcessOpenAiGpt5Nano(string s)
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
    var openAiClient = new OpenAIClient(apiKey);

    // Получаем ChatClient для модели
    ChatClient chatClient = openAiClient.GetChatClient("gpt-5-nano");

    var sw = Stopwatch.StartNew();

    var messages = new List<ChatMessage>
    {
        ChatMessage.CreateUserMessage(s)
    };

    ClientResult<ChatCompletion> openAiResponse = await chatClient.CompleteChatAsync(messages);

    sw.Stop();

    string answer = openAiResponse.Value.Content.First().Text;

    long inputTokens = openAiResponse.Value.Usage?.InputTokenCount ?? -1;
    long outputTokens = openAiResponse.Value.Usage?.OutputTokenCount ?? -1;

    DumpResult("OPENAI GPT 5 NANO RESULT", sw.ElapsedMilliseconds, inputTokens, outputTokens, answer);
}

async Task ProcessOpenAiGpt5Dot1(string s)
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
    var openAiClient = new OpenAIClient(apiKey);

    // Получаем ChatClient для модели
    ChatClient chatClient = openAiClient.GetChatClient("gpt-5.1");

    var sw = Stopwatch.StartNew();

    var messages = new List<ChatMessage>
    {
        ChatMessage.CreateUserMessage(s)
    };

    ClientResult<ChatCompletion> openAiResponse = await chatClient.CompleteChatAsync(messages);

    sw.Stop();

    string answer = openAiResponse.Value.Content.First().Text;

    long inputTokens = openAiResponse.Value.Usage?.InputTokenCount ?? -1;
    long outputTokens = openAiResponse.Value.Usage?.OutputTokenCount ?? -1;

    DumpResult("OPENAI GPT 5.1 RESULT", sw.ElapsedMilliseconds, inputTokens, outputTokens, answer);
}

void DumpResult(
    string header,
    long elapsedMs,
    long inputTokens,
    long outputTokens,
    string answer)
{
    Console.WriteLine("======================================");
    Console.WriteLine($"========== {header} =============");
    Console.WriteLine("======================================");
    Console.WriteLine($"Response time: {elapsedMs} ms");
    Console.WriteLine($"Tokens request: {inputTokens}");
    Console.WriteLine($"Tokens response: {outputTokens}");
    Console.WriteLine("\nResponse:\n");
    Console.WriteLine(answer);
    Console.WriteLine("\n\n");
}