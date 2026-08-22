using System.IO;
using CursorFrancais.Core;

namespace CursorFrancais.App.Services;

public sealed class StartupShortcut
{
    public string CheminRaccourci { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Startup),
        "Cursor Francais Companion.lnk");

    public bool EstPresent => File.Exists(CheminRaccourci);

    public void Appliquer(bool activer)
    {
        if (activer)
        {
            Creer();
        }
        else
        {
            Retirer();
        }
    }

    public void Creer()
    {
        var exe = Environment.ProcessPath
                  ?? Path.Combine(AppContext.BaseDirectory, "CursorFrancais.App.exe");
        var type = Type.GetTypeFromProgID("WScript.Shell")
                   ?? throw new InvalidOperationException(
                       "Impossible de créer le raccourci de démarrage. Réessayez après un redémarrage de l’explorateur.");
        var instance = Activator.CreateInstance(type)
                       ?? throw new InvalidOperationException("WScript.Shell introuvable.");
        dynamic shell = instance;
        dynamic raccourci = shell.CreateShortcut(CheminRaccourci);
        raccourci.TargetPath = exe;
        raccourci.WorkingDirectory = Path.GetDirectoryName(exe);
        raccourci.Description = CoreInfo.NomProduit;
        raccourci.Save();
    }

    public void Retirer()
    {
        if (File.Exists(CheminRaccourci))
        {
            File.Delete(CheminRaccourci);
        }
    }
}
