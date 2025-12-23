namespace Domain;

using System.Text.Json.Serialization;

public class Chunk
{
    [JsonPropertyName("chunkid")]
    public string ChunkId { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("embedding")]
    public float[] Embedding { get; set; } = [];
}