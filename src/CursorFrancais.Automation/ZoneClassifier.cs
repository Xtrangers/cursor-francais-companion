using CursorFrancais.Core;

namespace CursorFrancais.Automation;

public enum UiZone
{
    Chrome,
    Agent,
    Chat,
    Composer,
    Settings,
}

public static class ZoneClassifier
{
    public static UiZone Classer(UiElementHit hit, string titreFenetre)
    {
        var nom = hit.Nom;
        if (Contient(nom, "Settings") || Contient(nom, "Appearance") || Contient(nom, "Privacy")
            || Contient(nom, "Keyboard Shortcuts"))
        {
            return UiZone.Settings;
        }

        if (Contient(nom, "Composer") || Contient(nom, "Add to Composer") || Contient(nom, "Inline Edit"))
        {
            return UiZone.Composer;
        }

        if (Contient(nom, "New Chat") || Contient(nom, "Chat actions") || Contient(nom, "Copy message")
            || Contient(nom, "Fork chat") || nom.Equals("Chat", StringComparison.OrdinalIgnoreCase))
        {
            return UiZone.Chat;
        }

        if (Contient(nom, "Agent") || Contient(titreFenetre, "Agents"))
        {
            return UiZone.Agent;
        }

        return UiZone.Chrome;
    }

    public static bool Autorise(UiZone zone, UiSettings reglages) => zone switch
    {
        UiZone.Agent or UiZone.Chat => reglages.TranslateAgentChat,
        UiZone.Composer => reglages.TranslateComposer,
        UiZone.Settings => reglages.TranslateSettings,
        _ => true,
    };

    private static bool Contient(string texte, string morceau) =>
        texte.Contains(morceau, StringComparison.OrdinalIgnoreCase);
}
