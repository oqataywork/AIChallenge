using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Integrations.Embedding;

namespace Integrations.Ollama;

public class OllamaClient : IOllamaClient
{
    private readonly HttpClient _ollamaClient;
    private readonly string _ollamaUrl = "http://localhost:11434/api/embeddings";
    private readonly string _model = "nomic-embed-text";

    public OllamaClient()
    {
        _ollamaClient = new HttpClient();
    }

    public async Task<float[]> GetEmbedding(string text, CancellationToken ct)
    {
        var payload = new { model = _model, prompt = text };
        string json = JsonSerializer.Serialize(payload);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _ollamaClient.PostAsync(_ollamaUrl, content, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: ct);

        return result.Embedding;
    }

    public class OllamaEmbeddingResponse
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}