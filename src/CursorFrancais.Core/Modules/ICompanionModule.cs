namespace CursorFrancais.Core;

public interface ICompanionModule
{
    string Id { get; }

    string Title { get; }

    string Description { get; }

    bool IsAvailable { get; }

    object CreateView();
}
