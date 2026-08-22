namespace CursorFrancais.Overlay;

public readonly record struct LabelBox(int X, int Y, int Largeur, int Hauteur);

public sealed record OverlayLabel(string Texte, LabelBox Source);

public sealed record PlacedLabel(string Texte, LabelBox Box);

public static class LabelLayout
{
    public static IReadOnlyList<PlacedLabel> Placer(IEnumerable<OverlayLabel> labels, double taillePolice)
    {
        var places = new List<PlacedLabel>();
        foreach (var label in labels.OrderBy(l => l.Source.Y).ThenBy(l => l.Source.X))
        {
            var largeur = Math.Max(28, (int)Math.Ceiling(label.Texte.Length * taillePolice * 0.62) + 10);
            var hauteur = (int)Math.Ceiling(taillePolice + 8);
            var x = label.Source.X;
            var y = label.Source.Y - hauteur - 2;
            if (y < 0)
            {
                y = label.Source.Y + 2;
            }

            var box = new LabelBox(x, y, largeur, hauteur);
            var garde = 0;
            while (places.Any(p => Chevauche(p.Box, box)) && garde++ < 12)
            {
                box = box with { X = box.X + 10, Y = Math.Max(0, box.Y - 8) };
            }

            places.Add(new PlacedLabel(label.Texte, box));
        }

        return places;
    }

    public static bool Chevauche(LabelBox a, LabelBox b) =>
        a.X < b.X + b.Largeur
        && b.X < a.X + a.Largeur
        && a.Y < b.Y + b.Hauteur
        && b.Y < a.Y + a.Hauteur;
}
