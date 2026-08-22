using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorFrancais.Core;
using Microsoft.Win32;

namespace CursorFrancais.App.ViewModels;

public sealed partial class DictionaryViewModel : ObservableObject
{
    private readonly DictionaryStore _store;
    private readonly DictionaryEngine _moteur;
    private IReadOnlyList<DictionaryEntry> _toutes = [];

    public DictionaryViewModel(DictionaryStore store, DictionaryEngine moteur)
    {
        _store = store;
        _moteur = moteur;
        Etat = new PageStateViewModel();
        Charger();
    }

    public PageStateViewModel Etat { get; }

    public ObservableCollection<DictionaryEntry> Entrees { get; } = [];

    public IReadOnlyList<string> Categories { get; } =
        ["Toutes", "Menus", "Buttons", "Agent", "Settings", "System"];

    [ObservableProperty]
    private string filtreCategorie = "Toutes";

    [ObservableProperty]
    private string nouveauEn = string.Empty;

    [ObservableProperty]
    private string nouveauFr = string.Empty;

    [ObservableProperty]
    private string nouvelleCategorie = "Buttons";

    [ObservableProperty]
    private DictionaryEntry? selection;

    partial void OnFiltreCategorieChanged(string value) => AppliquerFiltre();

    [RelayCommand]
    private void Charger()
    {
        Etat.Chargement("Lecture du dictionnaire…");
        try
        {
            _toutes = _store.Lister();
            _moteur.Remplacer(_toutes);
            AppliquerFiltre();
            if (_toutes.Count == 0)
            {
                Etat.Vide("Dictionnaire vide", "Importez un JSON ou ajoutez un terme.");
            }
            else
            {
                Etat.Pret();
            }
        }
        catch (Exception ex)
        {
            Etat.Erreur($"Le dictionnaire n’a pas pu être lu : {ex.Message}. Réessayez.", ChargerCommand);
        }
    }

    [RelayCommand]
    private void Ajouter()
    {
        if (string.IsNullOrWhiteSpace(NouveauEn) || string.IsNullOrWhiteSpace(NouveauFr))
        {
            Etat.Erreur("Indiquez un terme anglais et sa traduction, puis réessayez.", AjouterCommand);
            return;
        }

        try
        {
            _store.Ajouter(NouveauEn, NouveauFr, NouvelleCategorie);
            NouveauEn = string.Empty;
            NouveauFr = string.Empty;
            Charger();
        }
        catch (Exception ex)
        {
            Etat.Erreur($"L’ajout a échoué : {ex.Message}. Réessayez.", AjouterCommand);
        }
    }

    [RelayCommand]
    private void Supprimer()
    {
        if (Selection is null)
        {
            return;
        }

        _store.Supprimer(Selection.Id);
        Charger();
    }

    [RelayCommand]
    private void Exporter()
    {
        var dialogue = new SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json",
            FileName = "dictionnaire-fr.json",
            Title = "Exporter le dictionnaire",
        };
        if (dialogue.ShowDialog() != true)
        {
            return;
        }

        File.WriteAllText(dialogue.FileName, SeedImporter.Ecrire(_store.Lister()));
    }

    [RelayCommand]
    private void Importer()
    {
        var dialogue = new OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json",
            Title = "Importer un dictionnaire",
        };
        if (dialogue.ShowDialog() != true)
        {
            return;
        }

        try
        {
            _store.Importer(SeedImporter.LireFichier(dialogue.FileName));
            Charger();
        }
        catch (Exception ex)
        {
            Etat.Erreur($"L’import a échoué : {ex.Message}. Vérifiez le fichier puis réessayez.", ImporterCommand);
        }
    }

    private void AppliquerFiltre()
    {
        Entrees.Clear();
        foreach (var entree in _toutes)
        {
            if (FiltreCategorie is "Toutes" ||
                string.Equals(entree.Category, FiltreCategorie, StringComparison.OrdinalIgnoreCase))
            {
                Entrees.Add(entree);
            }
        }
    }
}
