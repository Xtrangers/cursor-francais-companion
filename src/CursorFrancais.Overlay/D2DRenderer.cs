using System.Drawing;
using Vortice;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace CursorFrancais.Overlay;

public sealed class D2DRenderer : IDisposable
{
    private readonly ID2D1Factory _factory;
    private readonly IDWriteFactory _write;
    private ID2D1DCRenderTarget? _cible;
    private int _largeur;
    private int _hauteur;

    public D2DRenderer()
    {
        _factory = D2D1.D2D1CreateFactory<ID2D1Factory>();
        _write = DWrite.DWriteCreateFactory<IDWriteFactory>();
    }

    public void Dessiner(
        nint hdc,
        int largeur,
        int hauteur,
        IReadOnlyList<PlacedLabel> labels,
        string badge,
        double opacite,
        double taillePolice)
    {
        if (largeur <= 0 || hauteur <= 0)
        {
            return;
        }

        AssurerCible(largeur, hauteur);
        if (_cible is null)
        {
            return;
        }

        _cible.BindDC(hdc, new RawRect(0, 0, largeur, hauteur));
        _cible.BeginDraw();
        _cible.Clear(new Color4(0, 0, 0, 0));

        var alpha = (float)Math.Clamp(opacite, 0.5, 1.0);
        using var fond = _cible.CreateSolidColorBrush(new Color4(0.08f, 0.09f, 0.12f, alpha));
        using var texte = _cible.CreateSolidColorBrush(new Color4(0.95f, 0.96f, 0.97f, alpha));
        using var accent = _cible.CreateSolidColorBrush(new Color4(0.24f, 0.55f, 1f, alpha));
        using var format = _write.CreateTextFormat("Segoe UI", (float)taillePolice);
        format.WordWrapping = WordWrapping.NoWrap;
        var police = (float)taillePolice;

        foreach (var label in labels)
        {
            var pill = new RectangleF(label.Box.X, label.Box.Y, label.Box.Largeur, label.Box.Hauteur);
            var zone = new Rect(label.Box.X, label.Box.Y, label.Box.Largeur, label.Box.Hauteur);
            _cible.FillRoundedRectangle(new RoundedRectangle(pill, 4, 4), fond);
            _cible.DrawText(label.Texte, format, zone, texte);
        }

        if (!string.IsNullOrWhiteSpace(badge))
        {
            var largeurBadge = Math.Max(140f, badge.Length * police * 0.62f + 16f);
            var hauteurBadge = police + 10f;
            var pill = new RectangleF(8, 8, largeurBadge, hauteurBadge);
            var zone = new Rect(8, 8, largeurBadge, hauteurBadge);
            _cible.FillRoundedRectangle(new RoundedRectangle(pill, 6, 6), accent);
            _cible.DrawText(badge, format, zone, texte);
        }

        _cible.EndDraw();
    }

    public void Dispose()
    {
        _cible?.Dispose();
        _write.Dispose();
        _factory.Dispose();
    }

    private void AssurerCible(int largeur, int hauteur)
    {
        if (_cible is not null && _largeur == largeur && _hauteur == hauteur)
        {
            return;
        }

        _cible?.Dispose();
        var props = new RenderTargetProperties
        {
            Type = RenderTargetType.Default,
            PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, Vortice.DCommon.AlphaMode.Premultiplied),
            DpiX = 96,
            DpiY = 96,
            Usage = RenderTargetUsage.GdiCompatible,
            MinLevel = FeatureLevel.Default,
        };
        _cible = _factory.CreateDCRenderTarget(props);
        _largeur = largeur;
        _hauteur = hauteur;
    }
}
