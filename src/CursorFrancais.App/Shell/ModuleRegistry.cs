using CursorFrancais.Core;

namespace CursorFrancais.App.Shell;

public sealed class ModuleRegistry
{
    public ModuleRegistry(IEnumerable<ICompanionModule> modules)
    {
        Modules = modules.ToList();
        if (Modules.Count == 0)
        {
            throw new InvalidOperationException("Aucun module enregistré.");
        }
    }

    public IReadOnlyList<ICompanionModule> Modules { get; }

    public ICompanionModule Trouver(string? id)
    {
        return Modules.FirstOrDefault(m =>
                   string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase))
               ?? Modules[0];
    }
}
