namespace CursorFrancais.Automation;

public sealed record CursorTarget(
    int ProcessId,
    string ExePath,
    string VersionFichier,
    bool EstCheminFiable,
    IReadOnlyList<CursorWindowInfo> Fenetres);

public sealed record CursorWindowInfo(
    string Titre,
    string Classe,
    int Gauche,
    int Haut,
    int Largeur,
    int Hauteur);
