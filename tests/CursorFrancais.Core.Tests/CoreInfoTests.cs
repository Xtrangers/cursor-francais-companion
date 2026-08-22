using CursorFrancais.Core;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Core.Tests;

public class CoreInfoTests
{
    [Fact]
    public void NomProduit_est_renseigne()
    {
        CoreInfo.NomProduit.Should().NotBeNullOrWhiteSpace();
    }
}
