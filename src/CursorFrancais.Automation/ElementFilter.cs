using CursorFrancais.Core;

namespace CursorFrancais.Automation;

public static class ElementFilter
{
    private static readonly HashSet<string> TypesAutorises = new(StringComparer.OrdinalIgnoreCase)
    {
        "Button", "MenuItem", "TabItem", "Hyperlink", "SplitButton",
    };

    private static readonly HashSet<string> DejaFrancais = new(StringComparer.Ordinal)
    {
        "Réduire", "Restaurer", "Fermer", "Agrandir", "Réessayer",
    };

    public static IReadOnlyList<UiElementHit> Filtrer(IEnumerable<UiElementHit> elements)
    {
        return elements.Where(EstTraduisible).ToList();
    }

    public static bool EstTraduisible(UiElementHit hit)
    {
        if (hit.EstEditeur || string.IsNullOrWhiteSpace(hit.Nom))
        {
            return false;
        }

        if (!TypesAutorises.Contains(hit.TypeControle))
        {
            return false;
        }

        if (DejaFrancais.Contains(hit.Nom.Trim()))
        {
            return false;
        }

        if (hit.Nom.Length > 40)
        {
            return false;
        }

        return !ExclusionRules.EstExclu(hit.Nom);
    }
}
