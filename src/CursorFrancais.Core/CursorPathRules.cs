namespace CursorFrancais.Core;

public static class CursorPathRules
{
    public static bool EstNomProcessusCursor(string? nomProcessus)
    {
        return string.Equals(nomProcessus, "Cursor", StringComparison.OrdinalIgnoreCase);
    }

    public static bool EstCheminFiable(string? chemin)
    {
        if (string.IsNullOrWhiteSpace(chemin))
        {
            return false;
        }

        var normalise = chemin.Replace('/', '\\').Trim();
        if (!normalise.EndsWith("\\Cursor.exe", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (normalise.Contains("\\Microsoft VS Code\\", StringComparison.OrdinalIgnoreCase)
            || normalise.EndsWith("\\Code.exe", StringComparison.OrdinalIgnoreCase)
            || normalise.Contains("\\VSCodium\\", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return normalise.Contains("\\Programs\\cursor\\", StringComparison.OrdinalIgnoreCase)
            || normalise.Contains("\\cursor\\Cursor.exe", StringComparison.OrdinalIgnoreCase);
    }
}
