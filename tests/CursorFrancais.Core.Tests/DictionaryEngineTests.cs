using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class DictionaryEngineTests
{
    private static DictionaryEngine Moteur()
    {
        var moteur = new DictionaryEngine();
        moteur.Remplacer(
        [
            new DictionaryEntry(1, "New Chat", "new chat", "Nouveau chat", "Agent", false),
            new DictionaryEntry(2, "Apply", "apply", "Appliquer", "Agent", false),
            new DictionaryEntry(3, "Reject", "reject", "Rejeter", "Agent", false),
            new DictionaryEntry(4, "Run", "run", "Exécuter", "Menus", false),
            new DictionaryEntry(5, "Settings", "settings", "Réglages", "Settings", false),
            new DictionaryEntry(6, "Hide Sidebar", "hide sidebar", "Masquer la barre latérale", "Buttons", false),
            new DictionaryEntry(7, "File", "file", "Fichier", "Menus", false),
            new DictionaryEntry(8, "Help", "help", "Aide", "Menus", false),
            new DictionaryEntry(9, "Search", "search", "Recherche", "Buttons", false),
            new DictionaryEntry(10, "Open Workspace", "open workspace", "Ouvrir un espace de travail", "Buttons", false),
            new DictionaryEntry(11, "Automations", "automations", "Automatisations", "Buttons", false),
            new DictionaryEntry(12, "Go Back", "go back", "Précédent", "Buttons", false),
            new DictionaryEntry(13, "Copy message", "copy message", "Copier le message", "Agent", false),
            new DictionaryEntry(14, "Fork chat", "fork chat", "Dupliquer le chat", "Agent", false),
            new DictionaryEntry(15, "Enter Full Screen", "enter full screen", "Plein écran", "Buttons", false),
        ]);
        return moteur;
    }

    [Theory]
    [InlineData("New Chat", "Nouveau chat")]
    [InlineData("Apply", "Appliquer")]
    [InlineData("Reject", "Rejeter")]
    [InlineData("Run", "Exécuter")]
    [InlineData("Settings", "Réglages")]
    [InlineData("Hide Sidebar", "Masquer la barre latérale")]
    [InlineData("File", "Fichier")]
    [InlineData("Help", "Aide")]
    [InlineData("Search", "Recherche")]
    [InlineData("Open Workspace", "Ouvrir un espace de travail")]
    [InlineData("Automations", "Automatisations")]
    [InlineData("Go Back", "Précédent")]
    [InlineData("Copy message", "Copier le message")]
    [InlineData("Fork chat", "Dupliquer le chat")]
    [InlineData("Enter Full Screen", "Plein écran")]
    public void Traduire_exact(string source, string fr)
    {
        var r = Moteur().Traduire(source);
        r.Unknown.Should().BeFalse();
        r.Translation.Should().Be(fr);
    }

    [Theory]
    [InlineData("New Chat...")]
    [InlineData("new chat")]
    [InlineData("&Apply")]
    [InlineData("Apply:")]
    [InlineData("  Run  ")]
    public void Traduire_normalise(string source)
    {
        Moteur().Traduire(source).Unknown.Should().BeFalse();
    }

    [Theory]
    [InlineData("Agent")]
    [InlineData("Composer")]
    [InlineData("Cursor")]
    [InlineData("Skill")]
    [InlineData("MCP")]
    public void Traduire_garde_anglais(string source)
    {
        var r = Moteur().Traduire(source);
        r.Unknown.Should().BeFalse();
        r.KeepEnglish.Should().BeTrue();
        r.Translation.Should().BeNull();
    }

    [Theory]
    [InlineData("gpt-4o")]
    [InlineData(@"C:\proj\a.cs")]
    [InlineData("npm test")]
    [InlineData("un texte inconnu xyz")]
    [InlineData("")]
    public void Traduire_inconnu_ou_exclu(string source)
    {
        Moteur().Traduire(source).Unknown.Should().BeTrue();
    }

    [Fact]
    public void Seed_couvre_au_moins_150_termes()
    {
        SeedImporter.LireIntegre().Count.Should().BeGreaterThanOrEqualTo(150);
    }
}
