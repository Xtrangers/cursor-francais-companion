using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CursorFrancais.App.Views;
using CursorFrancais.Core;

namespace CursorFrancais.App.Shell;

public sealed partial class ShellViewModel : ObservableObject
{
    private readonly ModuleRegistry _registre;
    private readonly UiSettingsStore _store;
    private readonly UiSettings _reglages;
    private readonly Dictionary<string, object> _vues = new(StringComparer.OrdinalIgnoreCase);

    [ObservableProperty]
    private ICompanionModule? selectedModule;

    [ObservableProperty]
    private object? currentView;

    public ShellViewModel(ModuleRegistry registre, UiSettingsStore store, UiSettings reglages)
    {
        _registre = registre;
        _store = store;
        _reglages = reglages;
        Modules = registre.Modules;
        SelectedModule = registre.Trouver(reglages.LastModuleId);
    }

    public IReadOnlyList<ICompanionModule> Modules { get; }

    public string BandeauNonAffilie =>
        "Projet communautaire — non affilié à Cursor / Anysphere.";

    public string TitreFenetre => CoreInfo.NomProduit;

    partial void OnSelectedModuleChanged(ICompanionModule? value)
    {
        if (value is null)
        {
            return;
        }

        CurrentView = ObtenirVue(value);
        _reglages.LastModuleId = value.Id;
        _store.Save(_reglages);
    }

    [RelayCommand]
    private void Selectionner(ICompanionModule? module)
    {
        if (module is not null)
        {
            SelectedModule = module;
        }
    }

    private object ObtenirVue(ICompanionModule module)
    {
        if (_vues.TryGetValue(module.Id, out var existante))
        {
            return existante;
        }

        object vue = module.IsAvailable
            ? module.CreateView()
            : new ComingSoonView { DataContext = module };
        _vues[module.Id] = vue;
        return vue;
    }
}
