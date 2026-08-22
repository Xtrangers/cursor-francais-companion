using System.Windows.Threading;
using CursorFrancais.App.ViewModels;
using CursorFrancais.Automation;
using CursorFrancais.Core;
using CursorFrancais.Native;
using CursorFrancais.Overlay;

namespace CursorFrancais.App.Services;

public sealed class TranslationCoordinator : IDisposable
{
    private readonly CursorLocator _locator;
    private readonly UiaReader _lecteur = new();
    private readonly DictionaryEngine _moteur;
    private readonly DictionaryStore _store;
    private readonly UiSettings _reglages;
    private readonly JournalViewModel _journal;
    private readonly OverlayHost _host = new();
    private readonly Dictionary<nint, FrameDiffer> _diffs = [];
    private readonly WindowTracker _tracker = new();
    private readonly DispatcherTimer _timer;
    private int _dernierCompte;

    public TranslationCoordinator(
        CursorLocator locator,
        DictionaryEngine moteur,
        DictionaryStore store,
        UiSettings reglages,
        JournalViewModel journal,
        TranslatorViewModel traducteur)
    {
        _locator = locator;
        _moteur = moteur;
        _store = store;
        _reglages = reglages;
        _journal = journal;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _timer.Tick += (_, _) => Rafraichir();
        _tracker.Destroyed += hwnd => _host.Retirer(hwnd);
        traducteur.TranslationChanged += (_, _) =>
        {
            foreach (var diff in _diffs.Values)
            {
                diff.Reinitialiser();
            }

            Rafraichir();
        };
    }

    public int DernierCompte => _dernierCompte;

    public void Demarrer() => _timer.Start();

    public void Dispose()
    {
        _timer.Stop();
        _tracker.Dispose();
        _host.Dispose();
    }

    public void Rafraichir()
    {
        if (!_reglages.TranslationEnabled)
        {
            _host.ToutMasquer();
            _dernierCompte = 0;
            return;
        }

        var cibles = _locator.Lister().Where(c => c.EstCheminFiable).ToList();
        var hwnds = new HashSet<nint>();
        foreach (var fenetre in cibles.SelectMany(c => c.Fenetres))
        {
            if (fenetre.Hwnd != 0)
            {
                hwnds.Add(fenetre.Hwnd);
            }
        }

        _host.RetirerAbsentes(hwnds);
        var premier = WindowGeometry.PremierPlan();
        if (ForegroundPolicy.DoitMasquer(_reglages.AutoHideWhenUnfocused, premier, hwnds))
        {
            _host.ToutMasquer();
            return;
        }

        var total = 0;
        foreach (var fenetre in cibles.SelectMany(c => c.Fenetres).Where(f => f.Hwnd != 0))
        {
            total += DessinerFenetre(fenetre);
        }

        _dernierCompte = total;
    }

    private int DessinerFenetre(CursorWindowInfo fenetre)
    {
        var cadre = WindowGeometry.Lire(fenetre.Hwnd);
        if (cadre is null)
        {
            return 0;
        }

        var hits = _lecteur.LireVisibles(fenetre.Hwnd, TimeSpan.FromMilliseconds(40));
        var filtres = ElementFilter.Filtrer(hits);
        if (!_diffs.TryGetValue(fenetre.Hwnd, out var differ))
        {
            differ = new FrameDiffer();
            _diffs[fenetre.Hwnd] = differ;
        }

        var labels = new List<OverlayLabel>();
        foreach (var hit in filtres)
        {
            var zone = ZoneClassifier.Classer(hit, fenetre.Titre);
            if (!ZoneClassifier.Autorise(zone, _reglages))
            {
                continue;
            }

            var match = _moteur.Traduire(hit.Nom);
            if (match.Unknown)
            {
                _store.NoterInconnu(hit.Nom, zone.ToString());
                continue;
            }

            if (match.KeepEnglish || string.IsNullOrWhiteSpace(match.Translation))
            {
                continue;
            }

            var rect = CoordinateMapper.VersOverlay(hit, cadre.Value.Gauche, cadre.Value.Haut);
            var texte = _reglages.BilingualMode
                ? $"{match.Translation} — {hit.Nom}"
                : match.Translation!;
            labels.Add(new OverlayLabel(texte, new LabelBox(rect.X, rect.Y, rect.Largeur, rect.Hauteur)));
        }

        if (!differ.AChange(filtres) && labels.Count == 0)
        {
            return 0;
        }

        var places = LabelLayout.Placer(labels, _reglages.LabelFontSize);
        var overlay = _host.Obtenir(fenetre.Hwnd);
        overlay.Afficher(true);
        overlay.Presenter(
            cadre.Value,
            places,
            $"Traduction ON · {places.Count}",
            _reglages.OverlayOpacity,
            _reglages.LabelFontSize);
        return places.Count;
    }
}
