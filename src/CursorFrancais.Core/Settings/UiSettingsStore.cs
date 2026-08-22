using System.Text.Json;

namespace CursorFrancais.Core;

public sealed class UiSettingsStore
{
    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly AppPaths _chemins;
    private readonly object _verrou = new();

    public UiSettingsStore(AppPaths chemins)
    {
        _chemins = chemins;
    }

    public UiSettings Load()
    {
        lock (_verrou)
        {
            _chemins.AssurerDossiers();
            if (!File.Exists(_chemins.FichierReglages))
            {
                var vierge = new UiSettings();
                vierge.Normaliser();
                return vierge;
            }

            try
            {
                var json = File.ReadAllText(_chemins.FichierReglages);
                var lu = JsonSerializer.Deserialize<UiSettings>(json, Json) ?? new UiSettings();
                lu.Normaliser();
                return lu;
            }
            catch (Exception)
            {
                var repli = new UiSettings();
                repli.Normaliser();
                return repli;
            }
        }
    }

    public void Save(UiSettings reglages)
    {
        ArgumentNullException.ThrowIfNull(reglages);
        lock (_verrou)
        {
            reglages.Normaliser();
            _chemins.AssurerDossiers();
            var json = JsonSerializer.Serialize(reglages, Json);
            var tmp = _chemins.FichierReglages + ".tmp";
            File.WriteAllText(tmp, json);
            File.Copy(tmp, _chemins.FichierReglages, overwrite: true);
            File.Delete(tmp);
        }
    }
}
