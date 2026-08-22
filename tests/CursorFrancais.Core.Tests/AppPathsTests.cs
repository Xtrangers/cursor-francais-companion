using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class AppPathsTests
{
    [Fact]
    public void Racine_personnalisee_place_reglages_et_dictionnaire()
    {
        var dossier = Path.Combine(Path.GetTempPath(), "cfc-paths", Guid.NewGuid().ToString("N"));
        var chemins = new AppPaths(dossier);
        chemins.AssurerDossiers();

        chemins.FichierReglages.Should().StartWith(dossier);
        chemins.FichierDictionnaire.Should().EndWith("dict.db");
        Directory.Exists(chemins.DossierJournaux).Should().BeTrue();

        Directory.Delete(dossier, recursive: true);
    }

    [Fact]
    public void ModuleIds_sont_stables_pour_la_persistance()
    {
        ModuleIds.Traducteur.Should().Be("traducteur");
        ModuleIds.Skills.Should().Be("skills");
        ModuleIds.Projets.Should().Be("projets");
        ModuleIds.Agents.Should().Be("agents");
    }
}
