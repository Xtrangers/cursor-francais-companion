namespace CursorFrancais.Core;

public sealed class DictionaryEngine
{
    private static readonly HashSet<string> ToujoursAnglais = new(StringComparer.OrdinalIgnoreCase)
    {
        "Agent", "Composer", "Cursor", "Skill", "Skills", "MCP", "Tab",
    };

    private readonly Dictionary<string, DictionaryEntry> _exact = new(StringComparer.Ordinal);
    private readonly Dictionary<string, DictionaryEntry> _normalise = new(StringComparer.Ordinal);

    public int Count => _exact.Count;

    public void Remplacer(IEnumerable<DictionaryEntry> entrees)
    {
        _exact.Clear();
        _normalise.Clear();
        foreach (var entree in entrees)
        {
            _exact[entree.Source] = entree;
            _normalise[entree.Normalized] = entree;
        }
    }

    public MatchResult Traduire(string? source)
    {
        if (string.IsNullOrWhiteSpace(source) || ExclusionRules.EstExclu(source))
        {
            return MatchResult.None;
        }

        var brut = source.Trim();
        if (ToujoursAnglais.Contains(brut))
        {
            return MatchResult.Exact(brut, "System", keep: true);
        }

        if (_exact.TryGetValue(brut, out var exacte))
        {
            return MatchResult.Exact(exacte.Translation, exacte.Category, exacte.KeepEnglish);
        }

        var clef = TextNormalizer.Clef(brut);
        if (_normalise.TryGetValue(clef, out var normalisee))
        {
            return MatchResult.Exact(normalisee.Translation, normalisee.Category, normalisee.KeepEnglish);
        }

        return MatchResult.None;
    }
}
