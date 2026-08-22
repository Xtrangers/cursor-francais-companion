using System.Windows.Input;

namespace CursorFrancais.App.Services;

public readonly record struct HotkeySpec(ModifierKeys Modifiers, Key Key);

public static class HotkeyParser
{
    public static HotkeySpec Analyser(string? texte)
    {
        if (string.IsNullOrWhiteSpace(texte))
        {
            return new HotkeySpec(ModifierKeys.Control | ModifierKeys.Alt, Key.F);
        }

        var parts = texte.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var mods = ModifierKeys.None;
        var touche = Key.None;
        foreach (var part in parts)
        {
            if (part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase)
                || part.Equals("Control", StringComparison.OrdinalIgnoreCase))
            {
                mods |= ModifierKeys.Control;
            }
            else if (part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
            {
                mods |= ModifierKeys.Alt;
            }
            else if (part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
            {
                mods |= ModifierKeys.Shift;
            }
            else if (part.Equals("Win", StringComparison.OrdinalIgnoreCase)
                     || part.Equals("Windows", StringComparison.OrdinalIgnoreCase))
            {
                mods |= ModifierKeys.Windows;
            }
            else if (Enum.TryParse(part, ignoreCase: true, out Key parsed))
            {
                touche = parsed;
            }
        }

        if (touche == Key.None)
        {
            touche = Key.F;
        }

        if (mods == ModifierKeys.None)
        {
            mods = ModifierKeys.Control | ModifierKeys.Alt;
        }

        return new HotkeySpec(mods, touche);
    }

    public static string Formater(HotkeySpec spec)
    {
        var parts = new List<string>();
        if (spec.Modifiers.HasFlag(ModifierKeys.Control))
        {
            parts.Add("Ctrl");
        }

        if (spec.Modifiers.HasFlag(ModifierKeys.Alt))
        {
            parts.Add("Alt");
        }

        if (spec.Modifiers.HasFlag(ModifierKeys.Shift))
        {
            parts.Add("Shift");
        }

        if (spec.Modifiers.HasFlag(ModifierKeys.Windows))
        {
            parts.Add("Win");
        }

        parts.Add(spec.Key.ToString());
        return string.Join('+', parts);
    }
}
