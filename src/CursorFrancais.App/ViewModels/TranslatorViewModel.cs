using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorFrancais.App.Services;
using CursorFrancais.Core;

namespace CursorFrancais.App.ViewModels;

public sealed partial class TranslatorViewModel : ObservableObject
{
    private readonly UiSettingsStore _store;
    private readonly StartupShortcut _demarrage;

    [ObservableProperty]
    private string page = "accueil";

    public TranslatorViewModel(
        UiSettings reglages,
        UiSettingsStore store,
        CursorStatusService statut,
        StartupShortcut demarrage,
        DictionaryViewModel dictionnaire,
        JournalViewModel journal)
    {
        Reglages = reglages;
        _store = store;
        Statut = statut;
        _demarrage = demarrage;
        reglages.StartWithWindows = demarrage.EstPresent;
        Dictionnaire = dictionnaire;
        Journal = journal;
    }

    public DictionaryViewModel Dictionnaire { get; }

    public JournalViewModel Journal { get; }

    public UiSettings Reglages { get; }

    public CursorStatusService Statut { get; }

    public bool TranslationEnabled
    {
        get => Reglages.TranslationEnabled;
        set
        {
            if (Reglages.TranslationEnabled == value)
            {
                return;
            }

            Reglages.TranslationEnabled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LibelleActivation));
            Persister();
            TranslationChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool BilingualMode
    {
        get => Reglages.BilingualMode;
        set
        {
            if (Reglages.BilingualMode == value)
            {
                return;
            }

            Reglages.BilingualMode = value;
            OnPropertyChanged();
            Persister();
            TranslationChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TranslateAgentChat
    {
        get => Reglages.TranslateAgentChat;
        set { Reglages.TranslateAgentChat = value; Persister(); OnPropertyChanged(); }
    }

    public bool TranslateComposer
    {
        get => Reglages.TranslateComposer;
        set { Reglages.TranslateComposer = value; Persister(); OnPropertyChanged(); }
    }

    public bool TranslateSettingsZone
    {
        get => Reglages.TranslateSettings;
        set { Reglages.TranslateSettings = value; Persister(); OnPropertyChanged(); }
    }

    public bool AutoHideWhenUnfocused
    {
        get => Reglages.AutoHideWhenUnfocused;
        set { Reglages.AutoHideWhenUnfocused = value; Persister(); OnPropertyChanged(); }
    }

    public bool StartWithWindows
    {
        get => Reglages.StartWithWindows;
        set
        {
            Reglages.StartWithWindows = value;
            try
            {
                _demarrage.Appliquer(value);
                ErreurReglages = string.Empty;
            }
            catch (Exception ex)
            {
                ErreurReglages =
                    $"Le démarrage automatique a échoué : {ex.Message}. Décochez puis réessayez.";
            }

            Persister();
            OnPropertyChanged();
        }
    }

    public bool NeverSaveCaptures
    {
        get => Reglages.NeverSaveCaptures;
        set { Reglages.NeverSaveCaptures = value; Persister(); OnPropertyChanged(); }
    }

    public bool OcrEnabled
    {
        get => Reglages.OcrEnabled;
        set { Reglages.OcrEnabled = value; Persister(); OnPropertyChanged(); }
    }

    public double OverlayOpacityPercent
    {
        get => Math.Round(Reglages.OverlayOpacity * 100);
        set
        {
            Reglages.OverlayOpacity = value / 100.0;
            OnPropertyChanged();
            Persister();
            TranslationChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public double LabelFontSize
    {
        get => Reglages.LabelFontSize;
        set
        {
            Reglages.LabelFontSize = value;
            OnPropertyChanged();
            Persister();
            TranslationChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string Hotkey
    {
        get => Reglages.Hotkey;
        set
        {
            var spec = HotkeyParser.Analyser(value);
            Reglages.Hotkey = HotkeyParser.Formater(spec);
            OnPropertyChanged();
            Persister();
            HotkeyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string TranslationMode
    {
        get => Reglages.TranslationMode;
        set
        {
            Reglages.TranslationMode = value;
            OnPropertyChanged();
            Persister();
        }
    }

    public bool ModeIntelligent
    {
        get => TranslationMode == "intelligent";
        set { if (value) TranslationMode = "intelligent"; }
    }

    public bool ModeAutomatique
    {
        get => TranslationMode == "automatique";
        set { if (value) TranslationMode = "automatique"; }
    }

    public bool ModeManuel
    {
        get => TranslationMode == "manuel";
        set { if (value) TranslationMode = "manuel"; }
    }

    public string LibelleActivation =>
        TranslationEnabled ? "Traduction activée" : "Traduction désactivée";

    [ObservableProperty]
    private string erreurReglages = string.Empty;

    public event EventHandler? TranslationChanged;

    public event EventHandler? HotkeyChanged;

    [RelayCommand]
    private void Aller(string? cible)
    {
        if (!string.IsNullOrWhiteSpace(cible))
        {
            Page = cible;
        }
    }

    [RelayCommand]
    private void BasculerTraduction()
    {
        TranslationEnabled = !TranslationEnabled;
    }

    [RelayCommand]
    private void ReessayerStatut()
    {
        Statut.Rafraichir();
    }

    public void Persister()
    {
        _store.Save(Reglages);
    }
}
