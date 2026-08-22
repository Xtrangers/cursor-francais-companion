using System.Runtime.InteropServices;
using CursorFrancais.Native;

namespace CursorFrancais.Automation;

public sealed class WindowTracker : IDisposable
{
    private const uint EventSystemForeground = 0x0003;
    private const uint EventObjectDestroy = 0x8001;
    private const uint EventObjectLocationChange = 0x800B;
    private const uint WineventOutofcontext = 0;

    private readonly WinEventDelegate _rappel;
    private readonly nint _hook;
    private bool _disposed;

    public WindowTracker()
    {
        _rappel = OnEvent;
        _hook = SetWinEventHook(
            EventSystemForeground,
            EventObjectLocationChange,
            0,
            _rappel,
            0,
            0,
            WineventOutofcontext);
    }

    public event Action<nint>? Changed;

    public event Action<nint>? Destroyed;

    public event Action<nint>? ForegroundChanged;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_hook != 0)
        {
            UnhookWinEvent(_hook);
        }
    }

    private void OnEvent(nint hWinEventHook, uint eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        if (hwnd == 0 || idObject != 0)
        {
            return;
        }

        switch (eventType)
        {
            case EventObjectDestroy:
                Destroyed?.Invoke(hwnd);
                break;
            case EventSystemForeground:
                ForegroundChanged?.Invoke(hwnd);
                Changed?.Invoke(hwnd);
                break;
            case EventObjectLocationChange:
                if (WindowGeometry.EstVivante(hwnd))
                {
                    Changed?.Invoke(hwnd);
                }

                break;
        }
    }

    private delegate void WinEventDelegate(
        nint hWinEventHook,
        uint eventType,
        nint hwnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime);

    [DllImport("user32.dll")]
    private static extern nint SetWinEventHook(
        uint eventMin,
        uint eventMax,
        nint hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(nint hWinEventHook);
}
