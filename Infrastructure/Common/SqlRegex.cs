using System.Text.RegularExpressions;

namespace Infrastructure.Common;

public static partial class SqlRegex
{
    [GeneratedRegex(@"\.(?<fileName>\w+)\.sql$", RegexOptions.Compiled)]
    public static partial Regex FileNameRegexGenerator();
}