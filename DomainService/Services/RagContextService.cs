using System.Text.Json;

using Domain;

using DomainService.Contracts;

using Integrations.Embedding;

namespace DomainService.Services;

public class RagContextService : IRagContextService
{
    private const string EMBEDDINGS_PATH = @"C:\Users\ogtay\Downloads\Aliyev_Ogtay_embeddings.json";
    private readonly IOllamaClient _ollamaClient;

    public RagContextService(IOllamaClient ollamaClient)
    {
        _ollamaClient = ollamaClient;
    }

    public async Task<List<AiContext>> GetRagContext(string question, float similarityThreshold, CancellationToken ct)
    {
        if (!File.Exists(EMBEDDINGS_PATH))
        {
            Console.WriteLine("❌ JSON с embeddings не найден, использую обычный контекст");

            return new List<AiContext>(); // или fallback логика
        }

        try
        {
            string json = await File.ReadAllTextAsync(EMBEDDINGS_PATH, ct);
            List<Chunk> chunks = JsonSerializer.Deserialize<List<Chunk>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

            Console.WriteLine($"📖 Загружено {chunks.Count} chunks");

            float[] questionEmbedding = await _ollamaClient.GetEmbedding(question, ct);

            var relevantChunks = chunks
                .Select(
                    chunk => new
                    {
                        Chunk = chunk,
                        Score = CosineSimilarity(chunk.Embedding, questionEmbedding)
                    })
                .Where(x => x.Score > similarityThreshold)
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToList();

            Console.WriteLine($"🔍 Найдено {relevantChunks.Count} релевантных chunks (top: {relevantChunks.FirstOrDefault()?.Score:F3})");

            return relevantChunks.Select(
                    rc => new AiContext(
                        question: rc.Chunk.Text,
                        answer: $"[chunk_id:{rc.Chunk.ChunkId}] relevance:{rc.Score:F3}",
                        responseSource: ResponseSource.Rag))
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ RAG exception: {ex.Message}");

            return new List<AiContext>();
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
}