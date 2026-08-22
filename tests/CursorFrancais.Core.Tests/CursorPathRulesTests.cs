using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class CursorPathRulesTests
{
    [Fact]
    public void Nom_Cursor_est_accepte()
    {
        CursorPathRules.EstNomProcessusCursor("Cursor").Should().BeTrue();
    }

    [Fact]
    public void Nom_Code_est_refuse()
    {
        CursorPathRules.EstNomProcessusCursor("Code").Should().BeFalse();
    }

    [Theory]
    [InlineData(@"C:\Users\Rems\AppData\Local\Programs\cursor\Cursor.exe")]
    [InlineData(@"C:\Program Files\cursor\Cursor.exe")]
    public void Chemin_Cursor_officiel_est_accepte(string chemin)
    {
        CursorPathRules.EstCheminFiable(chemin).Should().BeTrue();
    }

    [Theory]
    [InlineData(@"C:\Users\Rems\AppData\Local\Programs\Microsoft VS Code\Code.exe")]
    [InlineData(@"C:\Program Files\Microsoft VS Code\Code.exe")]
    [InlineData(@"C:\Program Files\VSCodium\VSCodium.exe")]
    [InlineData(@"C:\autre\Notepad.exe")]
    [InlineData("")]
    [InlineData(null)]
    public void Chemins_etrangers_sont_refuses(string? chemin)
    {
        CursorPathRules.EstCheminFiable(chemin).Should().BeFalse();
    }
}
