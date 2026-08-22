namespace CursorFrancais.Ocr;

public sealed class OcrGovernor
{
    public const int MaxParSeconde = 2;

    private DateTimeOffset _dernier = DateTimeOffset.MinValue;

    public bool Active { get; set; }

    public double SeuilCpu { get; set; } = 30;

    public bool PeutLancer(double cpuOverlayPourcent)
    {
        if (!Active)
        {
            return false;
        }

        if (cpuOverlayPourcent > SeuilCpu)
        {
            return false;
        }

        var maintenant = DateTimeOffset.UtcNow;
        if ((maintenant - _dernier).TotalMilliseconds < 1000.0 / MaxParSeconde)
        {
            return false;
        }

        _dernier = maintenant;
        return true;
    }

    public void Reinitialiser() => _dernier = DateTimeOffset.MinValue;
}
