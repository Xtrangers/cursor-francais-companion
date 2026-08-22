using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class UiSettingsStoreTests
{
    [Fact]
    public void Save_puis_Load_conserve_les_valeurs()
    {
        var dossier = CreerDossier();
        try
        {
            var store = new UiSettingsStore(new AppPaths(dossier));
            var source = new UiSettings
            {
                TranslationEnabled = true,
                BilingualMode = true,
                OverlayOpacity = 0.7,
                LabelFontSize = 14,
                Hotkey = "Ctrl+Alt+T",
                LastModuleId = ModuleIds.Skills,
                AutoHideWhenUnfocused = false,
                TranslateComposer = false,
            };

            store.Save(source);
            var lu = store.Load();

            lu.TranslationEnabled.Should().BeTrue();
            lu.BilingualMode.Should().BeTrue();
            lu.OverlayOpacity.Should().Be(0.7);
            lu.LabelFontSize.Should().Be(14);
            lu.Hotkey.Should().Be("Ctrl+Alt+T");
            lu.LastModuleId.Should().Be(ModuleIds.Skills);
            lu.AutoHideWhenUnfocused.Should().BeFalse();
            lu.TranslateComposer.Should().BeFalse();
        }
        finally
        {
            Directory.Delete(dossier, recursive: true);
        }
    }

    [Fact]
    public void Load_sans_fichier_rend_les_defauts()
    {
        var dossier = CreerDossier();
        try
        {
            var lu = new UiSettingsStore(new AppPaths(dossier)).Load();
            lu.TranslationEnabled.Should().BeFalse();
            lu.StartWithWindows.Should().BeFalse();
            lu.LastModuleId.Should().Be(ModuleIds.Traducteur);
            lu.Hotkey.Should().Be("Ctrl+Alt+F");
            lu.NeverSaveCaptures.Should().BeTrue();
        }
        finally
        {
            Directory.Delete(dossier, recursive: true);
        }
    }

    [Fact]
    public void Normaliser_borne_opacite_et_taille()
    {
        var reglages = new UiSettings
        {
            OverlayOpacity = 0.1,
            LabelFontSize = 99,
            Hotkey = " ",
            LastModuleId = "",
            TranslationMode = "inconnu",
        };

        reglages.Normaliser();

        reglages.OverlayOpacity.Should().Be(0.5);
        reglages.LabelFontSize.Should().Be(18);
        reglages.Hotkey.Should().Be("Ctrl+Alt+F");
        reglages.LastModuleId.Should().Be(ModuleIds.Traducteur);
        reglages.TranslationMode.Should().Be("intelligent");
    }

    private static string CreerDossier()
    {
        var dossier = Path.Combine(Path.GetTempPath(), "CursorFrancaisTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dossier);
        return dossier;
    }
}
