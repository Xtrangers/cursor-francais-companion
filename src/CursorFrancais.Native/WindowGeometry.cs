using System.Runtime.InteropServices;

namespace CursorFrancais.Native;

public readonly record struct CadreFenetre(int Gauche, int Haut, int Largeur, int Hauteur);

public static class WindowGeometry
{
    private const int DwmwaExtendedFrameBounds = 9;

    public static CadreFenetre? Lire(nint hwnd)
    {
        if (hwnd == 0 || !IsWindow(hwnd))
        {
            return null;
        }

        if (DwmGetWindowAttribute(hwnd, DwmwaExtendedFrameBounds, out var dwm, Marshal.SizeOf<RECT>()) == 0)
        {
            return new CadreFenetre(dwm.Left, dwm.Top, dwm.Right - dwm.Left, dwm.Bottom - dwm.Top);
        }

        if (GetWindowRect(hwnd, out var classique))
        {
            return new CadreFenetre(
                classique.Left,
                classique.Top,
                classique.Right - classique.Left,
                classique.Bottom - classique.Top);
        }

        return null;
    }

    public static bool EstVivante(nint hwnd) => hwnd != 0 && IsWindow(hwnd) && IsWindowVisible(hwnd);

    public static nint PremierPlan() => GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(nint hWnd);

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(nint hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
