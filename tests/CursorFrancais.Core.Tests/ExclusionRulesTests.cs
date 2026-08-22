using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class ExclusionRulesTests
{
    [Theory]
    [InlineData(@"C:\proj\a.cs")]
    [InlineData("main.ts")]
    [InlineData("npm test")]
    [InlineData("git status")]
    [InlineData("dotnet build")]
    [InlineData("gpt-4o")]
    [InlineData("claude-4-sonnet")]
    [InlineData("Context 89%")]
    [InlineData("Editing 8 files")]
    [InlineData("Branch main")]
    [InlineData("un titre beaucoup trop long pour un bouton chrome Cursor interface")]
    public void EstExclu_protege_code_chemins_et_compteurs(string texte)
    {
        ExclusionRules.EstExclu(texte).Should().BeTrue();
    }

    [Theory]
    [InlineData("New Chat")]
    [InlineData("Apply")]
    [InlineData("Hide Sidebar")]
    public void EstExclu_laisse_passer_le_chrome(string texte)
    {
        ExclusionRules.EstExclu(texte).Should().BeFalse();
    }
}
