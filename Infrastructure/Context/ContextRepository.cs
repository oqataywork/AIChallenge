using System.Data;

using Dapper;

using Domain;

using Infrastructure.Common;

using Repository;

namespace Infrastructure.Context;

public class ContextRepository : IContextRepository
{
    private readonly IDbConnection _dbConnection;

    public ContextRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<AiContext>> GetAllContext(CancellationToken cancellationToken)
    {
        CommandDefinition command = CommandDefinitionExtensions.Create<ContextRepository>(nameof(GetAllContext), cancellationToken);

        IEnumerable<AiContextDb> result = await _dbConnection.QueryAsync<AiContextDb>(command);

        return result
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task AddContext(string question, string answer, CancellationToken cancellationToken)
    {
        AiContextDb aiContextDb = AiContextDb.FromDomain(question, answer);

        CommandDefinition command = CommandDefinitionExtensions.Create<ContextRepository>(nameof(AddContext), aiContextDb, cancellationToken);

        await _dbConnection.ExecuteAsync(command);
    }
}