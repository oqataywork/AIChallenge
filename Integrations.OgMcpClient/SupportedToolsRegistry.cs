namespace Integrations.OgMcpClient;

public static class SupportedToolsRegistry
{
    private static readonly HashSet<string> _tools = new();

    public static void RegisterTool(string toolName)
    {
        _tools.Add(toolName);
    }

    public static void RegisterTool(string[] toolsName)
    {
        foreach (string tool in toolsName)
        {
            _tools.Add(tool);
        }
    }

    public static IReadOnlyCollection<string> Tools => _tools;
}