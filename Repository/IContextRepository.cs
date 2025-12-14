using Domain;

namespace Repository;

public interface IContextRepository
{
    Task<List<AiContext>> GetAllContext(CancellationToken cancellationToken);
    Task AddContext(string question, string answer, CancellationToken cancellationToken);
}