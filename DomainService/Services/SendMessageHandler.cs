using System.Text.Json;

using Domain;

using DomainService.Contracts;

using Integrations.Embedding;

using Repository;

namespace DomainService.Services;

public class SendMessageHandler
{
    private const string EMBEDDINGS_PATH = @"C:\Users\ogtay\Downloads\Aliyev_Ogtay_embeddings.json";

    private readonly IPromptBuilder _promptBuilder;
    private readonly MessageSender _messageSender;
    private readonly IContextRepository _contextRepository;
    private readonly IOllamaClient _ollamaClient;

    public SendMessageHandler(
        IContextRepository contextRepository,
        IPromptBuilder promptBuilder,
        MessageSender messageSender,
        IOllamaClient ollamaClient)
    {
        _contextRepository = contextRepository;
        _promptBuilder = promptBuilder;
        _messageSender = messageSender;
        _ollamaClient = ollamaClient;
    }

    public async Task<AiResponse> Handle(SendMessageRequestInternal request, CancellationToken cancellationToken)
    {
        _promptBuilder
            .WithUserPrompt(request.UserMessage)
            .WithTemperature(request.Temperature);

        if (request.WithContext is false)
        {
            return await SendWithoutContext(request, cancellationToken);
        }

        List<AiContext> context = await _contextRepository.GetAllContext(cancellationToken);

        if (request.UseRag)
        {
            List<AiContext> ragContext = await GetRagContext(request.UserMessage, cancellationToken);

            context.AddRange(ragContext);
        }

        string prompt = _promptBuilder
            .WithContext(context)
            .Build();

        AiResponse response = await _messageSender.SendMessage(prompt, request.ModelType, cancellationToken);

        await _contextRepository.AddContext(response.Question, response.Answer, cancellationToken);

        return response;
    }

    private async Task<List<AiContext>> GetRagContext(string question, CancellationToken ct)
    {
        if (!File.Exists(EMBEDDINGS_PATH))
        {
            Console.WriteLine("❌ JSON с embeddings не найден, использую обычный контекст");

            return await _contextRepository.GetAllContext(ct);
        }

        try
        {
            // 1. ЧТЕНИЕ JSON
            string json = await File.ReadAllTextAsync(EMBEDDINGS_PATH, ct);
            List<Chunk> chunks = JsonSerializer.Deserialize<List<Chunk>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();

            Console.WriteLine($"📖 Загружено {chunks.Count} чанков");

            // 2. EMBEDDING вопроса через Ollama
            float[] questionEmbedding = await _ollamaClient.GetEmbedding(question, ct);

            // 3. ПОИСК ТОП-5 чанков
            var relevantChunks = chunks
                .Select(
                    chunk => new
                    {
                        Chunk = chunk,
                        Score = CosineSimilarity(chunk.Embedding, questionEmbedding)
                    })
                .Where(x => x.Score > 0.2f) // фильтр релевантности
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToList();

            Console.WriteLine($"🔍 Найдено {relevantChunks.Count} релевантных чанков (top: {relevantChunks.FirstOrDefault()?.Score:F3})");

            // 4. AiContext для промпта
            return relevantChunks.Select(
                    rc => new AiContext(
                        question: rc.Chunk.Text,
                        answer: $"[chunk_id:{rc.Chunk.ChunkId}] relevance:{rc.Score:F3}"))
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ RAG ошибка: {ex.Message}");

            return await _contextRepository.GetAllContext(ct);
        }
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        float dot = 0, normA = 0, normB = 0;
        int minLen = Math.Min(a.Length, b.Length);

        for (var i = 0; i < minLen; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        return normA == 0 || normB == 0 ? 0f : dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
    }

    private async Task<AiResponse> SendWithoutContext(SendMessageRequestInternal request, CancellationToken ct)
    {
        string prompt = _promptBuilder.WithoutContext().Build();

        return await _messageSender.SendMessage(prompt, request.ModelType, ct);
    }
}