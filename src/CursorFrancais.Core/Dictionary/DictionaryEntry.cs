namespace CursorFrancais.Core;

public sealed record DictionaryEntry(
    long Id,
    string Source,
    string Normalized,
    string Translation,
    string Category,
    bool KeepEnglish);

public sealed record UnknownTerm(
    string Normalized,
    string Sample,
    string Zone,
    int Count,
    DateTimeOffset LastSeen);

public sealed record MatchResult(string? Translation, bool KeepEnglish, bool Unknown, string? Category)
{
    public static MatchResult None { get; } = new(null, false, true, null);

    public static MatchResult Exact(string fr, string category, bool keep) =>
        new(keep ? null : fr, keep, false, category);
}
