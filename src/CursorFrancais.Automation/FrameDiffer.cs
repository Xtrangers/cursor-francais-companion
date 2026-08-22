namespace CursorFrancais.Automation;

public sealed class FrameDiffer
{
    private string? _empreinte;

    public bool AChange(IEnumerable<UiElementHit> elements)
    {
        var empreinte = string.Join(
            '|',
            elements.Select(e => $"{e.Nom}\t{e.Gauche},{e.Haut},{e.Largeur},{e.Hauteur}"));
        if (empreinte == _empreinte)
        {
            return false;
        }

        _empreinte = empreinte;
        return true;
    }

    public void Reinitialiser() => _empreinte = null;
}
