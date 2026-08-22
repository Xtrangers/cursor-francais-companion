using System.Text.RegularExpressions;

namespace CursorFrancais.Core;

public static partial class TextNormalizer
{
    public static string Normaliser(string? texte)
    {
        if (string.IsNullOrWhiteSpace(texte))
        {
            return string.Empty;
        }

        var s = texte.Trim();
        s = AccelRegex().Replace(s, string.Empty);
        s = s.Replace("&&", "&", StringComparison.Ordinal);
        s = EspacesRegex().Replace(s, " ");
        s = s.Replace("…", "...", StringComparison.Ordinal);
        s = PointsRegex().Replace(s, string.Empty);
        s = s.Trim().TrimEnd(':', '—', '-').Trim();
        return s;
    }

    public static string Clef(string? texte) => Normaliser(texte).ToLowerInvariant();

    [GeneratedRegex("&(?!&)")]
    private static partial Regex AccelRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex EspacesRegex();

    [GeneratedRegex(@"\.{2,}$")]
    private static partial Regex PointsRegex();
}
