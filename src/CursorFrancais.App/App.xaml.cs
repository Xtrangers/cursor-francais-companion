using System.IO;
using System.Windows;
using CursorFrancais.App.Modules;
using CursorFrancais.App.Services;
using CursorFrancais.App.Shell;
using CursorFrancais.App.ViewModels;
using CursorFrancais.App.Views;
using CursorFrancais.App.Views.Translator;
using CursorFrancais.Automation;
using CursorFrancais.Core;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CursorFrancais.App;

public partial class App : Application
{
    private ServiceProvider? _services;
    private HotkeyService? _hotkey;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var chemins = new AppPaths();
        chemins.AssurerDossiers();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(chemins.DossierJournaux, "companion-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();
        Configurer(services, chemins);
        _services = services.BuildServiceProvider();

        var store = _services.GetRequiredService<UiSettingsStore>();
        var reglages = _services.GetRequiredService<UiSettings>();

        if (!reglages.DisclaimerAccepted)
        {
            var disclaimer = new DisclaimerWindow();
            var ok = disclaimer.ShowDialog() == true && disclaimer.Accepte;
            if (!ok)
            {
                Shutdown();
                return;
            }

            reglages.DisclaimerAccepted = true;
            store.Save(reglages);
        }

        var fenetre = _services.GetRequiredService<MainWindow>();
        _hotkey = _services.GetRequiredService<HotkeyService>();
        _hotkey.Attacher(fenetre);
        if (!_hotkey.Enregistrer(reglages.Hotkey))
        {
            Log.Warning("Raccourci {Hotkey} déjà pris. Changez-le dans Réglages.", reglages.Hotkey);
        }

        var traducteur = _services.GetRequiredService<TranslatorViewModel>();
        _hotkey.Declenche += (_, _) =>
            Dispatcher.Invoke(() => traducteur.BasculerTraductionCommand.Execute(null));
        traducteur.HotkeyChanged += (_, _) => _hotkey.Enregistrer(traducteur.Hotkey);

        fenetre.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _hotkey?.Dispose();
        _services?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static void Configurer(IServiceCollection services, AppPaths chemins)
    {
        services.AddSingleton(chemins);
        services.AddSingleton<UiSettingsStore>();
        services.AddSingleton(sp => sp.GetRequiredService<UiSettingsStore>().Load());
        services.AddSingleton<CursorLocator>();
        services.AddSingleton<CursorStatusService>();
        services.AddSingleton<StartupShortcut>();
        services.AddSingleton<HotkeyService>();
        services.AddSingleton<DictionaryStore>();
        services.AddSingleton<DictionaryEngine>(sp =>
        {
            var store = sp.GetRequiredService<DictionaryStore>();
            SeedImporter.AppliquerSiBesoin(store);
            var moteur = new DictionaryEngine();
            moteur.Remplacer(store.Lister());
            return moteur;
        });
        services.AddSingleton<DictionaryViewModel>();
        services.AddSingleton<JournalViewModel>();
        services.AddSingleton<TranslatorViewModel>();
        services.AddSingleton<ICompanionModule>(sp => new StaticModule(
            ModuleIds.Traducteur,
            "Traducteur",
            "Overlay de traduction de l’interface Cursor.",
            isAvailable: true,
            () => new TranslatorHostView
            {
                DataContext = sp.GetRequiredService<TranslatorViewModel>(),
            }));
        services.AddSingleton<ICompanionModule>(_ => new StaticModule(
            ModuleIds.Skills,
            "Skills",
            "Gérer les skills Cursor de l’utilisateur, sans injection.",
            isAvailable: false,
            () => new object()));
        services.AddSingleton<ICompanionModule>(_ => new StaticModule(
            ModuleIds.Projets,
            "Projets",
            "Profils de workspaces et presets du compagnon.",
            isAvailable: false,
            () => new object()));
        services.AddSingleton<ICompanionModule>(_ => new StaticModule(
            ModuleIds.Agents,
            "Agents",
            "Liste et presets d’agents, orchestration locale.",
            isAvailable: false,
            () => new object()));
        services.AddSingleton<ModuleRegistry>();
        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
