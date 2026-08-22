using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class TextNormalizerTests
{
    [Theory]
    [InlineData("New Chat...", "New Chat")]
    [InlineData("New Chat…", "New Chat")]
    [InlineData("&File", "File")]
    [InlineData("Save && Exit", "Save & Exit")]
    [InlineData("  Open   Folder  ", "Open Folder")]
    [InlineData("Settings:", "Settings")]
    [InlineData("", "")]
    public void Normaliser_nettoie_le_texte(string source, string attendu)
    {
        TextNormalizer.Normaliser(source).Should().Be(attendu);
    }

    [Fact]
    public void Clef_ignore_la_casse()
    {
        TextNormalizer.Clef("New Chat...").Should().Be("new chat");
    }
}
