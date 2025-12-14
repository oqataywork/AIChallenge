using Domain;

using DomainService.Contracts;

using Repository;

namespace DomainService.Services;

public class SendMessageHandler
{
    private readonly IPromptBuilder _promptBuilder;
    private readonly MessageSender _messageSender;
    private readonly IContextRepository _contextRepository;

    public SendMessageHandler(IContextRepository contextRepository, IPromptBuilder promptBuilder, MessageSender messageSender)
    {
        _contextRepository = contextRepository;
        _promptBuilder = promptBuilder;
        _messageSender = messageSender;
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

        string prompt = _promptBuilder
            .WithContext(context)
            .Build();

        AiResponse response = await _messageSender.SendMessage(prompt, request.ModelType, cancellationToken);

        await _contextRepository.AddContext(response.Question, response.Answer, cancellationToken);

        return response;
    }

    private async Task<AiResponse> SendWithoutContext(SendMessageRequestInternal request, CancellationToken cancellationToken)
    {
        string prompt = _promptBuilder
            .WithoutContext()
            .Build();

        return await _messageSender.SendMessage(prompt, request.ModelType, cancellationToken);
    }
}