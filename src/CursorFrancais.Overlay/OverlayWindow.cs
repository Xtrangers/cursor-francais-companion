using System.Runtime.InteropServices;
using CursorFrancais.Native;

namespace CursorFrancais.Overlay;

public sealed class OverlayWindow : IDisposable
{
    private const int WsExLayered = 0x00080000;
    private const int WsExTransparent = 0x00000020;
    private const int WsExNoActivate = 0x08000000;
    private const int WsExToolwindow = 0x00000080;
    private const int WsExTopmost = 0x00000008;
    private const int WsPopup = unchecked((int)0x80000000);
    private const int SwpNoActivate = 0x0010;
    private const int SwpShowWindow = 0x0040;
    private const int UlwAlpha = 0x00000002;
    private const int BiRgb = 0;
    private const int DibRgbColors = 0;
    private const nint HwndTopmost = -1;

    private static readonly WndProcDelegate WndProcKeepAlive = DefWindowProc;
    private static bool _classeEnregistree;

    private readonly D2DRenderer _rendu = new();
    private nint _hwnd;
    private bool _visible = true;

    public nint Handle => _hwnd;

    public void Creer()
    {
        EnregistrerClasse();
        var styleEx = WsExLayered | WsExTransparent | WsExNoActivate | WsExToolwindow | WsExTopmost;
        _hwnd = CreateWindowEx(
            styleEx,
            "CursorFrancaisOverlay",
            string.Empty,
            WsPopup,
            0,
            0,
            100,
            100,
            0,
            0,
            GetModuleHandle(null),
            0);
        if (_hwnd == 0)
        {
            throw new InvalidOperationException("Impossible de créer la fenêtre overlay. Relancez le compagnon.");
        }
    }

    public void Deplacer(CadreFenetre cadre)
    {
        if (_hwnd == 0)
        {
            return;
        }

        SetWindowPos(
            _hwnd,
            HwndTopmost,
            cadre.Gauche,
            cadre.Haut,
            Math.Max(1, cadre.Largeur),
            Math.Max(1, cadre.Hauteur),
            SwpNoActivate | SwpShowWindow);
    }

    public void Afficher(bool visible)
    {
        _visible = visible;
        if (_hwnd != 0)
        {
            ShowWindow(_hwnd, visible ? 8 : 0);
        }
    }

    public void Presenter(
        CadreFenetre cadre,
        IReadOnlyList<PlacedLabel> labels,
        string badge,
        double opacite,
        double taillePolice)
    {
        if (_hwnd == 0 || !_visible)
        {
            return;
        }

        Deplacer(cadre);
        var largeur = Math.Max(1, cadre.Largeur);
        var hauteur = Math.Max(1, cadre.Hauteur);
        var bmi = new BITMAPINFO
        {
            bmiHeader = new BITMAPINFOHEADER
            {
                biSize = Marshal.SizeOf<BITMAPINFOHEADER>(),
                biWidth = largeur,
                biHeight = -hauteur,
                biPlanes = 1,
                biBitCount = 32,
                biCompression = BiRgb,
            },
        };

        var hdcEcran = GetDC(0);
        var hdcMem = CreateCompatibleDC(hdcEcran);
        var dib = CreateDIBSection(hdcMem, ref bmi, DibRgbColors, out _, 0, 0);
        var ancien = SelectObject(hdcMem, dib);
        try
        {
            _rendu.Dessiner(hdcMem, largeur, hauteur, labels, badge, opacite, taillePolice);
            var taille = new SIZE { Cx = largeur, Cy = hauteur };
            var source = new POINT();
            var dest = new POINT { X = cadre.Gauche, Y = cadre.Haut };
            var blend = new BLENDFUNCTION
            {
                BlendOp = 0,
                BlendFlags = 0,
                SourceConstantAlpha = 255,
                AlphaFormat = 1,
            };
            UpdateLayeredWindow(_hwnd, hdcEcran, ref dest, ref taille, hdcMem, ref source, 0, ref blend, UlwAlpha);
        }
        finally
        {
            SelectObject(hdcMem, ancien);
            DeleteObject(dib);
            DeleteDC(hdcMem);
            ReleaseDC(0, hdcEcran);
        }
    }

    public void Dispose()
    {
        _rendu.Dispose();
        if (_hwnd != 0)
        {
            DestroyWindow(_hwnd);
            _hwnd = 0;
        }
    }

    private static void EnregistrerClasse()
    {
        if (_classeEnregistree)
        {
            return;
        }

        var wc = new WNDCLASSEX
        {
            cbSize = Marshal.SizeOf<WNDCLASSEX>(),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(WndProcKeepAlive),
            hInstance = GetModuleHandle(null),
            lpszClassName = "CursorFrancaisOverlay",
        };
        if (RegisterClassEx(ref wc) == 0)
        {
            throw new InvalidOperationException("Impossible d’enregistrer la classe overlay.");
        }

        _classeEnregistree = true;
    }

    private delegate nint WndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    private static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW")]
    private static extern nint CreateWindowEx(
        int dwExStyle,
        string lpClassName,
        string lpWindowName,
        int dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW")]
    private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll")]
    private static extern bool DestroyWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool UpdateLayeredWindow(
        nint hWnd,
        nint hdcDst,
        ref POINT pptDst,
        ref SIZE psize,
        nint hdcSrc,
        ref POINT pptSrc,
        int crKey,
        ref BLENDFUNCTION pblend,
        int dwFlags);

    [DllImport("user32.dll")]
    private static extern nint GetDC(nint hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(nint hWnd, nint hDC);

    [DllImport("gdi32.dll")]
    private static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern nint SelectObject(nint hdc, nint hgdiobj);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(nint ho);

    [DllImport("gdi32.dll")]
    private static extern nint CreateDIBSection(
        nint hdc,
        ref BITMAPINFO pbmi,
        int usage,
        out nint ppvBits,
        nint hSection,
        int offset);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? lpModuleName);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WNDCLASSEX
    {
        public int cbSize;
        public int style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public nint hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE
    {
        public int Cx;
        public int Cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public int bmiColors;
    }
}
