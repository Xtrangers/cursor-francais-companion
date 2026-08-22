using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CursorFrancais.App.ViewModels;

public sealed partial class PageStateViewModel : ObservableObject
{
    [ObservableProperty]
    private string titre = string.Empty;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private string etat = "vide";

    [ObservableProperty]
    private bool afficherReessayer;

    public IRelayCommand? ReessayerCommand { get; set; }

    public void Chargement(string message)
    {
        Etat = "chargement";
        Titre = "Chargement";
        Message = message;
        AfficherReessayer = false;
    }

    public void Vide(string titre, string message)
    {
        Etat = "vide";
        Titre = titre;
        Message = message;
        AfficherReessayer = false;
    }

    public void Erreur(string message, IRelayCommand reessayer)
    {
        Etat = "erreur";
        Titre = "Erreur";
        Message = message;
        ReessayerCommand = reessayer;
        AfficherReessayer = true;
        OnPropertyChanged(nameof(ReessayerCommand));
    }

    public void Pret()
    {
        Etat = "ok";
        AfficherReessayer = false;
    }
}
