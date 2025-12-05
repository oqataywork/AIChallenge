using System.Text.Json;

using DomainService;
using DomainService.Contracts;

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
    public async Task<AiResponse?> SendMessage([FromBody] SendMessageRequest request)
    {
        SendMessageRequestInternal requestInternal = SendMessageConverter.Convert(request);

        return await _sendMessageHandler.Handle(requestInternal);
    }
}