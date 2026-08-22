namespace CursorFrancais.Core;

public sealed class AppPaths
{
    public AppPaths(string? racine = null)
    {
        Racine = string.IsNullOrWhiteSpace(racine)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CursorFrancais")
            : racine;
    }

    public string Racine { get; }

    public string FichierReglages => Path.Combine(Racine, "ui-settings.json");

    public string FichierDictionnaire => Path.Combine(Racine, "dict.db");

    public string DossierJournaux => Path.Combine(Racine, "logs");

    public void AssurerDossiers()
    {
        Directory.CreateDirectory(Racine);
        Directory.CreateDirectory(DossierJournaux);
    }
}
