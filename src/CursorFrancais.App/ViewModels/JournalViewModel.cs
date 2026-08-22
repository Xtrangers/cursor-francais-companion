using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorFrancais.Core;

namespace CursorFrancais.App.ViewModels;

public sealed partial class JournalViewModel : ObservableObject
{
    private readonly DictionaryStore _store;

    public JournalViewModel(DictionaryStore store)
    {
        _store = store;
        Etat = new PageStateViewModel();
        Charger();
    }

    public PageStateViewModel Etat { get; }

    public ObservableCollection<UnknownTerm> Termes { get; } = [];

    public int Total => Termes.Sum(t => t.Count);

    [RelayCommand]
    private void Charger()
    {
        Etat.Chargement("Lecture du journal…");
        try
        {
            Termes.Clear();
            foreach (var terme in _store.ListerInconnus())
            {
                Termes.Add(terme);
            }

            OnPropertyChanged(nameof(Total));
            if (Termes.Count == 0)
            {
                Etat.Vide("Journal vide", "Aucun texte inconnu pour l’instant.");
            }
            else
            {
                Etat.Pret();
            }
        }
        catch (Exception ex)
        {
            Etat.Erreur($"Le journal n’a pas pu être lu : {ex.Message}. Réessayez.", ChargerCommand);
        }
    }
}
