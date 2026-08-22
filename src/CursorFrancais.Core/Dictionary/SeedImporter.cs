using System.Reflection;
using System.Text.Json;

namespace CursorFrancais.Core;

public static class SeedImporter
{
    public const string Version = "1";

    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public static IReadOnlyList<SeedTerm> LireIntegre()
    {
        var assemblee = typeof(SeedImporter).Assembly;
        var nom = assemblee.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("seed-fr.json", StringComparison.OrdinalIgnoreCase));
        if (nom is null)
        {
            throw new InvalidOperationException("Le fichier seed-fr.json est introuvable dans l’assembly.");
        }

        using var flux = assemblee.GetManifestResourceStream(nom)
                         ?? throw new InvalidOperationException("Impossible de lire seed-fr.json.");
        return JsonSerializer.Deserialize<List<SeedTerm>>(flux, Json) ?? [];
    }

    public static IReadOnlyList<SeedTerm> LireFichier(string chemin)
    {
        var json = File.ReadAllText(chemin);
        return JsonSerializer.Deserialize<List<SeedTerm>>(json, Json) ?? [];
    }

    public static string Ecrire(IEnumerable<DictionaryEntry> entrees)
    {
        var termes = entrees.Select(e => new SeedTerm
        {
            En = e.Source,
            Fr = e.Translation,
            Category = e.Category,
            KeepEnglish = e.KeepEnglish,
        }).ToList();
        return JsonSerializer.Serialize(termes, Json);
    }

    public static int AppliquerSiBesoin(DictionaryStore store)
    {
        if (store.EstSeedApplique())
        {
            return 0;
        }

        var n = store.Importer(LireIntegre());
        store.MarquerSeed(Version);
        return n;
    }
}
