using Domain;

using DomainService.Contracts;

using Presentation.Controllers.Contracts;

namespace Presentation.Controllers;

public static class SendMessageConverter
{
    public static SendMessageRequestInternal Convert(SendMessageRequest message)
    {
        return new SendMessageRequestInternal(
            message.UserMessage,
            message.WithContext,
            ConvertModel(message.Model),
            message.Temperature,
            message.UseRag);
    }

    public static SendMessageResponse Convert(AiResponse response)
    {
        return new SendMessageResponse(response.Answer, response.IsFinished);
    }

    private static ModelType ConvertModel(string model)
    {
        return model switch
        {
            "DeepSeekChat" => ModelType.DeepSeekChat,
            "DeepSeekReasoner" => ModelType.DeepSeekReasoner,
            "OpenAiGpt5Nano" => ModelType.OpenAiGpt5Nano,
            "OpenAiGpt5Dot1" => ModelType.OpenAiGpt5Dot1,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
        };
    }
}