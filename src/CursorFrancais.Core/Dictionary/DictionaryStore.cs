using Microsoft.Data.Sqlite;

namespace CursorFrancais.Core;

public sealed class DictionaryStore : IDisposable
{
    private readonly SqliteConnection _connexion;

    public DictionaryStore(AppPaths chemins)
    {
        chemins.AssurerDossiers();
        _connexion = new SqliteConnection($"Data Source={chemins.FichierDictionnaire};Pooling=False");
        _connexion.Open();
        AppliquerMigration();
    }

    public void Dispose() => _connexion.Dispose();

    public IReadOnlyList<DictionaryEntry> Lister()
    {
        using var cmd = _connexion.CreateCommand();
        cmd.CommandText =
            "SELECT id, source, normalized, translation, category, keep_english FROM entries ORDER BY source COLLATE NOCASE";
        var liste = new List<DictionaryEntry>();
        using var lecteur = cmd.ExecuteReader();
        while (lecteur.Read())
        {
            liste.Add(Lire(lecteur));
        }

        return liste;
    }

    public DictionaryEntry Ajouter(string source, string translation, string category, bool keepEnglish = false)
    {
        var normalise = TextNormalizer.Clef(source);
        using var cmd = _connexion.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO entries (source, normalized, translation, category, keep_english)
            VALUES ($s, $n, $t, $c, $k)
            ON CONFLICT(normalized) DO UPDATE SET
              source = excluded.source,
              translation = excluded.translation,
              category = excluded.category,
              keep_english = excluded.keep_english
            RETURNING id, source, normalized, translation, category, keep_english
            """;
        cmd.Parameters.AddWithValue("$s", source.Trim());
        cmd.Parameters.AddWithValue("$n", normalise);
        cmd.Parameters.AddWithValue("$t", translation.Trim());
        cmd.Parameters.AddWithValue("$c", category);
        cmd.Parameters.AddWithValue("$k", keepEnglish ? 1 : 0);
        using var lecteur = cmd.ExecuteReader();
        lecteur.Read();
        return Lire(lecteur);
    }

    public void Supprimer(long id)
    {
        using var cmd = _connexion.CreateCommand();
        cmd.CommandText = "DELETE FROM entries WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public int Importer(IEnumerable<SeedTerm> termes)
    {
        var n = 0;
        using var tx = _connexion.BeginTransaction();
        foreach (var terme in termes)
        {
            Ajouter(terme.En, terme.Fr, terme.Category, terme.KeepEnglish);
            n++;
        }

        tx.Commit();
        return n;
    }

    public IReadOnlyList<UnknownTerm> ListerInconnus()
    {
        using var cmd = _connexion.CreateCommand();
        cmd.CommandText =
            "SELECT normalized, sample, zone, count, last_seen FROM unknown_terms ORDER BY count DESC, last_seen DESC";
        var liste = new List<UnknownTerm>();
        using var lecteur = cmd.ExecuteReader();
        while (lecteur.Read())
        {
            liste.Add(new UnknownTerm(
                lecteur.GetString(0),
                lecteur.GetString(1),
                lecteur.GetString(2),
                lecteur.GetInt32(3),
                DateTimeOffset.Parse(lecteur.GetString(4))));
        }

        return liste;
    }

    public void NoterInconnu(string sample, string zone)
    {
        var normalise = TextNormalizer.Clef(sample);
        if (string.IsNullOrEmpty(normalise))
        {
            return;
        }

        using var cmd = _connexion.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO unknown_terms (normalized, sample, zone, count, last_seen)
            VALUES ($n, $s, $z, 1, $d)
            ON CONFLICT(normalized) DO UPDATE SET
              count = count + 1,
              sample = excluded.sample,
              zone = excluded.zone,
              last_seen = excluded.last_seen
            """;
        cmd.Parameters.AddWithValue("$n", normalise);
        cmd.Parameters.AddWithValue("$s", sample.Trim());
        cmd.Parameters.AddWithValue("$z", zone);
        cmd.Parameters.AddWithValue("$d", DateTimeOffset.UtcNow.ToString("O"));
        cmd.ExecuteNonQuery();
    }

    public bool EstSeedApplique()
    {
        using var cmd = _connexion.CreateCommand();
        cmd.CommandText = "SELECT value FROM meta WHERE key = 'seed_version'";
        return cmd.ExecuteScalar() is string;
    }

    public void MarquerSeed(string version)
    {
        using var cmd = _connexion.CreateCommand();
        cmd.CommandText =
            "INSERT INTO meta(key, value) VALUES ('seed_version', $v) ON CONFLICT(key) DO UPDATE SET value = excluded.value";
        cmd.Parameters.AddWithValue("$v", version);
        cmd.ExecuteNonQuery();
    }

    private void AppliquerMigration()
    {
        using var cmd = _connexion.CreateCommand();
        cmd.CommandText =
            """
            CREATE TABLE IF NOT EXISTS meta (
              key TEXT PRIMARY KEY,
              value TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS entries (
              id INTEGER PRIMARY KEY AUTOINCREMENT,
              source TEXT NOT NULL,
              normalized TEXT NOT NULL UNIQUE,
              translation TEXT NOT NULL,
              category TEXT NOT NULL,
              keep_english INTEGER NOT NULL DEFAULT 0
            );
            CREATE TABLE IF NOT EXISTS unknown_terms (
              normalized TEXT PRIMARY KEY,
              sample TEXT NOT NULL,
              zone TEXT NOT NULL,
              count INTEGER NOT NULL,
              last_seen TEXT NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    private static DictionaryEntry Lire(SqliteDataReader lecteur) =>
        new(
            lecteur.GetInt64(0),
            lecteur.GetString(1),
            lecteur.GetString(2),
            lecteur.GetString(3),
            lecteur.GetString(4),
            lecteur.GetInt32(5) != 0);
}
