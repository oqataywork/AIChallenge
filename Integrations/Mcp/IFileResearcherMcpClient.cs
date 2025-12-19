using Integrations.Mcp.Contracts;

namespace Integrations.Mcp;

public interface IFileResearcherMcpClient
{
    Task<string> ReadFile(ReadFileRequestDto request, CancellationToken cancellationToken);

    Task<string> CreateFile(CreateFileRequestDto request, CancellationToken cancellationToken);
}