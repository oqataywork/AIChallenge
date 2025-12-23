namespace Integrations.Embedding;

public interface IOllamaClient
{
    public Task<float[]> GetEmbedding(string text, CancellationToken ct);
}