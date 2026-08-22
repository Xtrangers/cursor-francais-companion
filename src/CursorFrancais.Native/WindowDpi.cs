using Windows.Win32;
using Windows.Win32.Foundation;

namespace CursorFrancais.Native;

public static class WindowDpi
{
    public static uint Lire(nint hwnd)
    {
        if (hwnd == 0)
        {
            return 96;
        }

#pragma warning disable CA1416
        var dpi = PInvoke.GetDpiForWindow(new HWND(hwnd));
#pragma warning restore CA1416
        return dpi == 0 ? 96u : dpi;
    }
}
