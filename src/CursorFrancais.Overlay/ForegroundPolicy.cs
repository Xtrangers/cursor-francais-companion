namespace CursorFrancais.Overlay;

public static class ForegroundPolicy
{
    public static bool DoitMasquer(
        bool autoHide,
        nint premierPlan,
        IReadOnlyCollection<nint> hwndCursor)
    {
        if (!autoHide)
        {
            return false;
        }

        if (premierPlan == 0)
        {
            return true;
        }

        return !hwndCursor.Contains(premierPlan);
    }
}
