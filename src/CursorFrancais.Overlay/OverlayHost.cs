using CursorFrancais.Native;

namespace CursorFrancais.Overlay;

public sealed class OverlayHost : IDisposable
{
    private readonly Dictionary<nint, OverlayWindow> _fenetres = [];

    public IReadOnlyCollection<nint> Cibles => _fenetres.Keys;

    public OverlayWindow Obtenir(nint hwndCursor)
    {
        if (_fenetres.TryGetValue(hwndCursor, out var existante))
        {
            return existante;
        }

        var overlay = new OverlayWindow();
        overlay.Creer();
        _fenetres[hwndCursor] = overlay;
        return overlay;
    }

    public void Retirer(nint hwndCursor)
    {
        if (_fenetres.Remove(hwndCursor, out var overlay))
        {
            overlay.Dispose();
        }
    }

    public void RetirerAbsentes(IReadOnlySet<nint> vivantes)
    {
        foreach (var hwnd in _fenetres.Keys.ToList())
        {
            if (!vivantes.Contains(hwnd) || !WindowGeometry.EstVivante(hwnd))
            {
                Retirer(hwnd);
            }
        }
    }

    public void ToutMasquer()
    {
        foreach (var overlay in _fenetres.Values)
        {
            overlay.Afficher(false);
        }
    }

    public void Dispose()
    {
        foreach (var overlay in _fenetres.Values)
        {
            overlay.Dispose();
        }

        _fenetres.Clear();
    }
}
