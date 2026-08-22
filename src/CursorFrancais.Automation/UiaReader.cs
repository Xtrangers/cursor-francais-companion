using System.Windows.Automation;

namespace CursorFrancais.Automation;

public sealed class UiaReader
{
    public IReadOnlyList<UiElementHit> LireVisibles(AutomationElement racine, int limite = 800)
    {
        var hits = new List<UiElementHit>();
        AutomationElementCollection descendants;
        try
        {
            descendants = racine.FindAll(TreeScope.Descendants, Condition.TrueCondition);
        }
        catch (Exception)
        {
            return hits;
        }

        foreach (AutomationElement element in descendants)
        {
            if (hits.Count >= limite)
            {
                break;
            }

            try
            {
                var type = element.Current.ControlType.ProgrammaticName.Replace(
                    "ControlType.",
                    string.Empty,
                    StringComparison.Ordinal);
                var nom = Tronquer(element.Current.Name);
                var rect = element.Current.BoundingRectangle;
                if (rect.IsEmpty || rect.Width < 2 || rect.Height < 2)
                {
                    continue;
                }

                hits.Add(new UiElementHit(
                    nom,
                    type,
                    Tronquer(element.Current.AutomationId),
                    Tronquer(element.Current.ClassName),
                    (int)rect.X,
                    (int)rect.Y,
                    (int)rect.Width,
                    (int)rect.Height,
                    EstEditeur(type, element.Current.ClassName)));
            }
            catch (ElementNotAvailableException)
            {
            }
        }

        return hits;
    }

    private static bool EstEditeur(string type, string classe)
    {
        return type is "Edit" or "Document"
            || classe.Contains("monaco", StringComparison.OrdinalIgnoreCase)
            || classe.Contains("editor", StringComparison.OrdinalIgnoreCase);
    }

    private static string Tronquer(string? valeur)
    {
        if (string.IsNullOrWhiteSpace(valeur))
        {
            return string.Empty;
        }

        var texte = valeur.Trim();
        return texte.Length <= 120 ? texte : texte[..117] + "...";
    }
}
