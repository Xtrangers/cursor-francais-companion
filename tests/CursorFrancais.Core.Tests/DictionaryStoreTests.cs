using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class DictionaryStoreTests
{
    [Fact]
    public void Migration_importe_et_round_trip_json()
    {
        var dossier = Path.Combine(Path.GetTempPath(), "cfc-dict", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dossier);
        try
        {
            using (var store = new DictionaryStore(new AppPaths(dossier)))
            {
                store.EstSeedApplique().Should().BeFalse();
                var n = SeedImporter.AppliquerSiBesoin(store);
                n.Should().BeGreaterThanOrEqualTo(150);
                store.EstSeedApplique().Should().BeTrue();
                SeedImporter.AppliquerSiBesoin(store).Should().Be(0);

                var json = SeedImporter.Ecrire(store.Lister());
                var reload = Path.Combine(dossier, "export.json");
                File.WriteAllText(reload, json);
                var importes = SeedImporter.LireFichier(reload);
                importes.Should().HaveCount(store.Lister().Count);

                store.NoterInconnu("Weird Button", "Agent");
                store.NoterInconnu("Weird Button", "Agent");
                store.ListerInconnus().Should().ContainSingle(t => t.Sample == "Weird Button" && t.Count == 2);
            }
        }
        finally
        {
            try
            {
                Directory.Delete(dossier, recursive: true);
            }
            catch (IOException)
            {
            }
        }
    }
}
