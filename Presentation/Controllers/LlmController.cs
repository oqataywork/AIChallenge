using Domain;

using DomainService.Contracts;
using DomainService.Services;

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
    public async Task<SendMessageResponse> SendMessage([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        SendMessageRequestInternal requestInternal = SendMessageConverter.Convert(request);

        AiResponse response = await _sendMessageHandler.Handle(requestInternal, cancellationToken);

        return SendMessageConverter.Convert(response);
    }
}