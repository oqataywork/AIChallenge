using Domain;

namespace DomainService.Contracts;

public record SendMessageRequestInternal(
    string UserMessage,
    bool WithContext,
    ModelType ModelType,
    double Temperature);