using Domain;

using Integrations.Contracts;

namespace DomainService.Services;

public static class SendMessageConverter
{
    public static SendMessageRequestDto Convert(string prompt)
    {
        return new SendMessageRequestDto(prompt);
    }

    public static AiResponse Convert(AiResponseDto responseDto)
    {
        return new AiResponse(responseDto.Question, responseDto.Answer, responseDto.IsFinished);
    }
}