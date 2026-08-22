using CursorFrancais.Core;

namespace CursorFrancais.Ocr;

public static class RoiFilter
{
    public static bool EstInterdit(string? texte, bool estEditeur, string? classe)
    {
        if (estEditeur)
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(classe)
            && (classe.Contains("monaco", StringComparison.OrdinalIgnoreCase)
                || classe.Contains("editor", StringComparison.OrdinalIgnoreCase)
                || classe.Contains("terminal", StringComparison.OrdinalIgnoreCase)
                || classe.Contains("console", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return ExclusionRules.EstExclu(texte);
    }
}
