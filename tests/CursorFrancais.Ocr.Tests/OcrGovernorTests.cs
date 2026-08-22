using CursorFrancais.Ocr;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Ocr.Tests;

public class OcrGovernorTests
{
    [Fact]
    public void Refuse_si_desactive_ou_cpu_haut()
    {
        var g = new OcrGovernor { Active = false };
        g.PeutLancer(1).Should().BeFalse();
        g.Active = true;
        g.PeutLancer(80).Should().BeFalse();
        g.PeutLancer(5).Should().BeTrue();
        g.PeutLancer(5).Should().BeFalse();
    }

    [Theory]
    [InlineData("function main()", false, "monaco-editor", true)]
    [InlineData("npm test", false, "", true)]
    [InlineData(@"C:\proj\a.cs", false, "", true)]
    [InlineData("Hide Sidebar", false, "", false)]
    [InlineData("New Chat", true, "", true)]
    public void Roi_exclut_editeur_et_commandes(string texte, bool editeur, string classe, bool interdit)
    {
        RoiFilter.EstInterdit(texte, editeur, classe).Should().Be(interdit);
    }
}
