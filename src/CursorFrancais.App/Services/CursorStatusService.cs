using CommunityToolkit.Mvvm.ComponentModel;
using CursorFrancais.Automation;

namespace CursorFrancais.App.Services;

public sealed partial class CursorStatusService : ObservableObject, IDisposable
{
    private readonly CursorLocator _locator;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(2));
    private readonly CancellationTokenSource _cts = new();

    [ObservableProperty]
    private string etat = "chargement";

    [ObservableProperty]
    private string message = "Recherche de Cursor…";

    [ObservableProperty]
    private string version = string.Empty;

    [ObservableProperty]
    private int fenetres;

    public CursorStatusService(CursorLocator locator)
    {
        _locator = locator;
        _ = BouclerAsync();
    }

    public void Rafraichir()
    {
        try
        {
            var cibles = _locator.Lister()
                .Where(c => c.EstCheminFiable)
                .ToList();
            if (cibles.Count == 0)
            {
                Etat = "vide";
                Message = "Cursor n’est pas détecté. Ouvrez Cursor, puis réessayez.";
                Version = string.Empty;
                Fenetres = 0;
                return;
            }

            var principale = cibles[0];
            Fenetres = cibles.Sum(c => c.Fenetres.Count);
            Version = principale.VersionFichier;
            Etat = "ok";
            Message = Fenetres == 0
                ? $"Cursor {Version} est lancé, aucune fenêtre visible."
                : $"Cursor {Version} — {Fenetres} fenêtre(s).";
        }
        catch (Exception ex)
        {
            Etat = "erreur";
            Message = $"La détection a échoué : {ex.Message}. Réessayez.";
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer.Dispose();
        _cts.Dispose();
    }

    private async Task BouclerAsync()
    {
        Rafraichir();
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token).ConfigureAwait(true))
            {
                Rafraichir();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
