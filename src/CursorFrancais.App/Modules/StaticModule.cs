using CursorFrancais.Core;

namespace CursorFrancais.App.Modules;

public sealed class StaticModule : ICompanionModule
{
    private readonly Func<object> _fabrique;

    public StaticModule(
        string id,
        string title,
        string description,
        bool isAvailable,
        Func<object> fabrique)
    {
        Id = id;
        Title = title;
        Description = description;
        IsAvailable = isAvailable;
        _fabrique = fabrique;
    }

    public string Id { get; }

    public string Title { get; }

    public string Description { get; }

    public bool IsAvailable { get; }

    public object CreateView() => _fabrique();
}
