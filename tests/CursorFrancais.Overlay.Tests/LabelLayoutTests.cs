using CursorFrancais.Overlay;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Overlay.Tests;

public class LabelLayoutTests
{
    [Fact]
    public void Deux_labels_ne_se_recouvrent_pas()
    {
        var places = LabelLayout.Placer(
        [
            new OverlayLabel("Fichier", new LabelBox(10, 20, 40, 16)),
            new OverlayLabel("Édition", new LabelBox(12, 22, 40, 16)),
        ],
        12);
        places.Should().HaveCount(2);
        LabelLayout.Chevauche(places[0].Box, places[1].Box).Should().BeFalse();
    }

    [Fact]
    public void Auto_hide_si_autre_application()
    {
        ForegroundPolicy.DoitMasquer(true, 99, [1, 2]).Should().BeTrue();
        ForegroundPolicy.DoitMasquer(true, 1, [1, 2]).Should().BeFalse();
        ForegroundPolicy.DoitMasquer(false, 99, [1, 2]).Should().BeFalse();
    }
}
