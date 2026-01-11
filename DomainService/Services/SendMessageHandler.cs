using Domain;

using DomainService.Contracts;


using Repository;

namespace DomainService.Services;

public class SendMessageHandler
{
    private readonly IPromptBuilder _promptBuilder;
    private readonly MessageSender _messageSender;
    private readonly IContextRepository _contextRepository;
    private readonly IRagContextService _ragContextService;

    public SendMessageHandler(
        IContextRepository contextRepository,
        IPromptBuilder promptBuilder,
        MessageSender messageSender,
        IRagContextService ragContextService)
    {
        _contextRepository = contextRepository;
        _promptBuilder = promptBuilder;
        _messageSender = messageSender;
        _ragContextService = ragContextService;
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
            List<AiContext> ragContext = await _ragContextService.GetRagContext(request.UserMessage, request.SimilarityThreshold, cancellationToken);

            context.AddRange(ragContext);
        }

        string prompt = _promptBuilder
            .WithContext(context)
            .Build();

        AiResponse response = await _messageSender.SendMessage(prompt, request.ModelType, cancellationToken);

        await _contextRepository.AddContext(response.Question, response.Answer, cancellationToken);

        return response;
    }

    private async Task<AiResponse> SendWithoutContext(SendMessageRequestInternal request, CancellationToken ct)
    {
        string prompt = _promptBuilder.WithoutContext().Build();

        return await _messageSender.SendMessage(prompt, request.ModelType, ct);
    }
}