namespace CursorFrancais.Core;

public sealed class UiSettings
{
    public bool TranslationEnabled { get; set; }

    public bool BilingualMode { get; set; }

    public string TranslationMode { get; set; } = "intelligent";

    public bool TranslateAgentChat { get; set; } = true;

    public bool TranslateComposer { get; set; } = true;

    public bool TranslateSettings { get; set; } = true;

    public bool AutoHideWhenUnfocused { get; set; } = true;

    public double OverlayOpacity { get; set; } = 0.92;

    public double LabelFontSize { get; set; } = 12;

    public string Hotkey { get; set; } = "Ctrl+Alt+F";

    public bool StartWithWindows { get; set; }

    public bool NeverSaveCaptures { get; set; } = true;

    public bool OcrEnabled { get; set; }

    public string LastModuleId { get; set; } = ModuleIds.Traducteur;

    public bool DisclaimerAccepted { get; set; }

    public void Normaliser()
    {
        OverlayOpacity = Math.Clamp(OverlayOpacity, 0.5, 1.0);
        LabelFontSize = Math.Clamp(LabelFontSize, 10, 18);
        if (string.IsNullOrWhiteSpace(Hotkey))
        {
            Hotkey = "Ctrl+Alt+F";
        }

        if (string.IsNullOrWhiteSpace(LastModuleId))
        {
            LastModuleId = ModuleIds.Traducteur;
        }

        if (TranslationMode is not ("intelligent" or "automatique" or "manuel"))
        {
            TranslationMode = "intelligent";
        }
    }
}
