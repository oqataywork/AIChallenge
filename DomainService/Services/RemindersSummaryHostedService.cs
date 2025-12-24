using Domain;

using DomainService.Contracts;

namespace DomainService.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class RemindersSummaryHostedService : BackgroundService
{
    private readonly ILogger<RemindersSummaryHostedService> _logger;
    private readonly SendMessageHandler _handler;

    public RemindersSummaryHostedService(
        ILogger<RemindersSummaryHostedService> logger,
        SendMessageHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new SendMessageRequestInternal(
                    UserMessage: "Сделай саммари напоминаний на 19.12.2025. Ответ дай на английском",
                    WithContext: true,
                    ModelType: ModelType.OpenAiGpt5Dot1,
                    Temperature: 0.7,
                    false,
                    0);

                AiResponse result = await _handler.Handle(request, stoppingToken);

                _logger.LogWarning(result.Answer);
            }
            catch (OperationCanceledException)
            {
                // graceful shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reminders summary job failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}