using Dapper;

namespace Infrastructure.Common;

internal class CommandDefinitionExtensions
{
    internal static CommandDefinition Create<T>(
        string scriptName,
        object parameters = default!,
        CancellationToken cancellationToken = default) where T : class
    {
        return new CommandDefinition(SqlScriptProvider<T>.Get(scriptName), parameters, cancellationToken: cancellationToken);
    }

    internal static CommandDefinition CreateWithTimeout<T>(
        string scriptName,
        object parameters,
        int timeOut,
        CancellationToken cancellationToken = default) where T : class
    {
        return new CommandDefinition(SqlScriptProvider<T>.Get(scriptName), parameters, commandTimeout: timeOut, cancellationToken: cancellationToken);
    }
}