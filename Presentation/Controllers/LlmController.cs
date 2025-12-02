using System.Text.Json;

using DomainService;

using Integrations.DeepSeek.Contracts;

using Microsoft.AspNetCore.Mvc;

using Presentation.Controllers.Contracts;

namespace Presentation.Controllers;

[ApiController]
public class LlmController : ControllerBase
{
    private readonly SendMessageHandler _sendMessageHandler;

    public LlmController(SendMessageHandler sendMessageHandler)
    {
        _sendMessageHandler = sendMessageHandler;
    }

    [HttpPost("SendMessage")]
    public async Task<AiResponse?> SendMessage([FromBody] SendMessageQuery message)
    {
        return await _sendMessageHandler.Handle(message.UserMessage);
    }
}