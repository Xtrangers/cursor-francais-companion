using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using CursorFrancais.Core;

namespace CursorFrancais.App.Services;

public sealed class HotkeyService : IDisposable
{
    public const int HotkeyMessageId = 0x0312;
    private const int HotkeyId = 0x46;

    private HwndSource? _source;
    private bool _enregistre;
    private nint _hwnd;

    public event EventHandler? Declenche;

    public void Attacher(Window fenetre)
    {
        ArgumentNullException.ThrowIfNull(fenetre);
        var helper = new WindowInteropHelper(fenetre);
        helper.EnsureHandle();
        _hwnd = helper.Handle;
        _source = HwndSource.FromHwnd(_hwnd);
        _source?.AddHook(Hook);
    }

    public bool Enregistrer(string raccourci)
    {
        if (_hwnd == 0)
        {
            return false;
        }

        Liberer();
        var spec = HotkeyParser.Analyser(raccourci);
        var vk = KeyInterop.VirtualKeyFromKey(spec.Key);
        var ok = RegisterHotKey(_hwnd, HotkeyId, VersModificateurs(spec.Modifiers), (uint)vk);
        _enregistre = ok;
        return ok;
    }

    public void Liberer()
    {
        if (_enregistre && _hwnd != 0)
        {
            UnregisterHotKey(_hwnd, HotkeyId);
            _enregistre = false;
        }
    }

    public void Dispose()
    {
        Liberer();
        _source?.RemoveHook(Hook);
        _source = null;
    }

    private nint Hook(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == HotkeyMessageId && wParam == HotkeyId)
        {
            Declenche?.Invoke(this, EventArgs.Empty);
            handled = true;
        }

        return 0;
    }

    private static uint VersModificateurs(ModifierKeys mods)
    {
        uint value = 0;
        if (mods.HasFlag(ModifierKeys.Alt))
        {
            value |= 0x0001;
        }

        if (mods.HasFlag(ModifierKeys.Control))
        {
            value |= 0x0002;
        }

        if (mods.HasFlag(ModifierKeys.Shift))
        {
            value |= 0x0004;
        }

        if (mods.HasFlag(ModifierKeys.Windows))
        {
            value |= 0x0008;
        }

        return value;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);
}
