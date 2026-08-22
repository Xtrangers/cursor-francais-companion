using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace CursorFrancais.Ocr;

public sealed class CursorFrame : IDisposable
{
    public CursorFrame(SoftwareBitmap bitmap)
    {
        Bitmap = bitmap;
    }

    public SoftwareBitmap Bitmap { get; }

    public void Dispose() => Bitmap.Dispose();
}

public static class CursorFrameGrabber
{
    private const uint PwRenderFullContent = 2;

    public static async Task<CursorFrame?> CapturerAsync(nint hwnd, CancellationToken annulation)
    {
        if (hwnd == 0)
        {
            return null;
        }

        if (!GetClientRect(hwnd, out var rect) || rect.Right <= 0 || rect.Bottom <= 0)
        {
            return null;
        }

        var largeur = rect.Right;
        var hauteur = rect.Bottom;
        var hdcFenetre = GetDC(hwnd);
        var hdcMem = CreateCompatibleDC(hdcFenetre);
        var hbm = CreateCompatibleBitmap(hdcFenetre, largeur, hauteur);
        var ancien = SelectObject(hdcMem, hbm);
        try
        {
            if (!PrintWindow(hwnd, hdcMem, PwRenderFullContent))
            {
                return null;
            }

            using var flux = new InMemoryRandomAccessStream();
            await CopierBitmapAsync(hbm, largeur, hauteur, flux, annulation).ConfigureAwait(false);
            flux.Seek(0);
            var decoder = await BitmapDecoder.CreateAsync(flux);
            var logiciel = await decoder.GetSoftwareBitmapAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied);
            return new CursorFrame(logiciel);
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            SelectObject(hdcMem, ancien);
            DeleteObject(hbm);
            DeleteDC(hdcMem);
            ReleaseDC(hwnd, hdcFenetre);
        }
    }

    private static Task CopierBitmapAsync(nint hbm, int largeur, int hauteur, IRandomAccessStream destination, CancellationToken annulation)
    {
        return Task.Run(
            () =>
            {
                annulation.ThrowIfCancellationRequested();
                using var gdi = System.Drawing.Image.FromHbitmap(hbm);
                using var memoire = new MemoryStream();
                gdi.Save(memoire, System.Drawing.Imaging.ImageFormat.Png);
                memoire.Position = 0;
                using var sortie = destination.AsStreamForWrite();
                memoire.CopyTo(sortie);
            },
            annulation);
    }

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(nint hwnd, nint hdcBlt, uint nFlags);

    [DllImport("user32.dll")]
    private static extern nint GetDC(nint hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(nint hWnd, nint hDC);

    [DllImport("gdi32.dll")]
    private static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern nint CreateCompatibleBitmap(nint hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    private static extern nint SelectObject(nint hdc, nint hgdiobj);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(nint ho);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(nint hdc);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
