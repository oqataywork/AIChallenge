namespace Presentation.Controllers.Contracts;

public class SendMessageRequest
{
    public string UserMessage { get; set; }
    public bool WithContext { get; set; }
    public string Model { get; set; }
    public double Temperature { get; set; }

    public bool UseRag { get; set; }

    public float SimilarityThreshold { get; set; }
}