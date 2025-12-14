using System.Reflection;

namespace Infrastructure.Common;

public static class SqlScriptProvider<T> where T : class
{
    private static readonly Dictionary<string, string> Scripts;

    static SqlScriptProvider()
    {
        Type repositoryType = typeof(T);
        Assembly asm = repositoryType.Assembly;
        Scripts = asm.GetManifestResourceNames()
            .Where<string>((Func<string, bool>)(x => x.StartsWith(repositoryType.Namespace)))
            .ToDictionary<string, string, string>(
                ClearManifestResourceName,
                (Func<string, string>)(v => LoadEmbeddedScript(asm, v)));
    }

    public static string Get(string key)
    {
        return Scripts[key];
    }

    private static string LoadEmbeddedScript(Assembly asm, string v)
    {
        using Stream? manifestResourceStream = asm.GetManifestResourceStream(v);

        if (manifestResourceStream == null)
        {
            throw new FileNotFoundException("Unable to find the SQL file from an embedded resource", v);
        }

        using (var streamReader = new StreamReader(manifestResourceStream))
        {
            return streamReader.ReadToEnd();
        }
    }

    private static string ClearManifestResourceName(string src)
    {
        return SqlRegex.FileNameRegexGenerator().Match(src).Groups["fileName"].Value;
    }
}