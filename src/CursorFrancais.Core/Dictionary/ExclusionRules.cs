using System.Text.RegularExpressions;

namespace CursorFrancais.Core;

public static partial class ExclusionRules
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs", ".ts", ".tsx", ".js", ".jsx", ".json", ".md", ".py", ".go", ".rs",
        ".java", ".cpp", ".h", ".xaml", ".csproj", ".sln", ".yml", ".yaml",
    };

    private static readonly string[] PrefixeCommandes =
    [
        "npm ", "npx ", "yarn ", "pnpm ", "dotnet ", "git ", "cargo ", "pip ",
    ];

    public static bool EstExclu(string? texte)
    {
        if (string.IsNullOrWhiteSpace(texte))
        {
            return true;
        }

        var brut = texte.Trim();
        if (brut.Length > 64)
        {
            return true;
        }

        if (CheminRegex().IsMatch(brut))
        {
            return true;
        }

        if (ModeleRegex().IsMatch(brut))
        {
            return true;
        }

        if (CompteurRegex().IsMatch(brut))
        {
            return true;
        }

        if (BrancheRegex().IsMatch(brut))
        {
            return true;
        }

        var extension = Path.GetExtension(brut);
        if (!string.IsNullOrEmpty(extension) && Extensions.Contains(extension))
        {
            return true;
        }

        foreach (var prefixe in PrefixeCommandes)
        {
            if (brut.StartsWith(prefixe, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    [GeneratedRegex(@"^[A-Za-z]:\\|^\\\\|/home/|/Users/|\\")]
    private static partial Regex CheminRegex();

    [GeneratedRegex(@"\b(gpt-[\w.-]+|o[1-9](-\w+)?|claude-[\w.-]+|gemini-[\w.-]+|cursor-small)\b", RegexOptions.IgnoreCase)]
    private static partial Regex ModeleRegex();

    [GeneratedRegex(@"\d+\s*%|Editing \d+|Context \d+", RegexOptions.IgnoreCase)]
    private static partial Regex CompteurRegex();

    [GeneratedRegex(@"^Branch\s", RegexOptions.IgnoreCase)]
    private static partial Regex BrancheRegex();
}
