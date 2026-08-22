namespace CursorFrancais.Automation;

public readonly record struct OverlayRect(int X, int Y, int Largeur, int Hauteur);

public static class CoordinateMapper
{
    public static OverlayRect VersOverlay(UiElementHit hit, int origineX, int origineY)
    {
        return new OverlayRect(
            hit.Gauche - origineX,
            hit.Haut - origineY,
            hit.Largeur,
            hit.Hauteur);
    }

    public static double Echelle(uint dpi) => dpi / 96.0;

    public static int Arrondi(double valeur) => (int)Math.Round(valeur, MidpointRounding.AwayFromZero);

    public static OverlayRect AjusterDpi(OverlayRect rect, uint dpiSource, uint dpiCible)
    {
        if (dpiSource == 0 || dpiCible == 0 || dpiSource == dpiCible)
        {
            return rect;
        }

        var facteur = dpiCible / (double)dpiSource;
        return new OverlayRect(
            Arrondi(rect.X * facteur),
            Arrondi(rect.Y * facteur),
            Arrondi(rect.Largeur * facteur),
            Arrondi(rect.Hauteur * facteur));
    }
}
