namespace CursorFrancais.Automation;

public sealed record UiElementHit(
    string Nom,
    string TypeControle,
    string AutomationId,
    string Classe,
    int Gauche,
    int Haut,
    int Largeur,
    int Hauteur,
    bool EstEditeur);
