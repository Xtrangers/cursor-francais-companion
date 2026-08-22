using System.IO;
using System.Text.Json;
using CursorFrancais.Automation;
using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Automation.Tests;

public class ElementFilterTests
{
    [Fact]
    public void Filtre_dump_fige_garde_le_chrome_et_protege_lediteur()
    {
        var hits = LireFixture();
        var gardes = ElementFilter.Filtrer(hits);
        gardes.Select(h => h.Nom).Should().BeEquivalentTo("File", "New Chat", "Hide Sidebar", "Add to Composer");
        gardes.Should().NotContain(h => h.EstEditeur);
        gardes.Should().NotContain(h => h.Nom.Contains("conversation", StringComparison.Ordinal));
        gardes.Should().NotContain(h => h.Nom == "gpt-4o");
        gardes.Should().NotContain(h => h.Nom == "Réduire");
    }

    [Fact]
    public void Zone_respecte_les_cases()
    {
        var hits = LireFixture();
        var reglages = new UiSettings { TranslateAgentChat = true, TranslateComposer = false, TranslateSettings = true };
        var autorises = hits.Where(h =>
            ElementFilter.EstTraduisible(h) &&
            ZoneClassifier.Autorise(ZoneClassifier.Classer(h, "Cursor Agents"), reglages));
        autorises.Select(h => h.Nom).Should().NotContain("Add to Composer");
        autorises.Select(h => h.Nom).Should().Contain("New Chat");
    }

    [Fact]
    public void Mapper_dpi_reste_dans_2px_a_150_pourcent()
    {
        var source = new OverlayRect(100, 40, 80, 20);
        var a150 = CoordinateMapper.AjusterDpi(source, 96, 144);
        a150.X.Should().Be(150);
        a150.Y.Should().Be(60);
        CoordinateMapper.Echelle(144).Should().BeApproximately(1.5, 0.001);
        CoordinateMapper.Echelle(120).Should().BeApproximately(1.25, 0.001);
        CoordinateMapper.Echelle(96).Should().Be(1);
    }

    [Fact]
    public void Differ_ignore_une_frame_identique()
    {
        var hits = LireFixture();
        var differ = new FrameDiffer();
        differ.AChange(hits).Should().BeTrue();
        differ.AChange(hits).Should().BeFalse();
        differ.AChange(hits.Take(3)).Should().BeTrue();
    }

    private static List<UiElementHit> LireFixture()
    {
        var chemin = Path.Combine(AppContext.BaseDirectory, "Fixtures", "chrome-sample.json");
        if (!File.Exists(chemin))
        {
            chemin = Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "Fixtures",
                "chrome-sample.json");
        }

        var json = File.ReadAllText(chemin);
        return JsonSerializer.Deserialize<List<UiElementHit>>(json)
               ?? throw new InvalidOperationException("Fixture UIA illisible.");
    }
}
