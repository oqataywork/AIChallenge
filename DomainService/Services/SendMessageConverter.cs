using Domain;

using Integrations.Ai.Contracts;

namespace DomainService.Services;

public static class SendMessageConverter
{
    public static SendMessageRequestDto Convert(string prompt)
    {
        return new SendMessageRequestDto(prompt);
    }

    public static AiResponse Convert(AiResponseDto responseDto)
    {
        return new AiResponse(responseDto.Question, responseDto.Answer, responseDto.IsFinished, ConvertResponseSource(responseDto.ResponseSource));
    }

    private static ResponseSource ConvertResponseSource(string contextSource)
    {
        return contextSource switch
        {
            "Db" => ResponseSource.Db,
            "Rag" => ResponseSource.Rag,
            "Model" => ResponseSource.Model,
            _ => throw new ArgumentOutOfRangeException(nameof(contextSource), contextSource, null)
        };
    }
}