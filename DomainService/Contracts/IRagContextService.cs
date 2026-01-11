using Domain;

namespace DomainService.Contracts;

public interface IRagContextService
{
    Task<List<AiContext>> GetRagContext(string question, float similarityThreshold, CancellationToken cancellationToken);
}