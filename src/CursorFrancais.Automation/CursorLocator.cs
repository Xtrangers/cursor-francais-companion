using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using CursorFrancais.Core;

namespace CursorFrancais.Automation;

public sealed class CursorLocator
{
    public IReadOnlyList<CursorTarget> Lister()
    {
        var resultats = new List<CursorTarget>();
        foreach (var processus in Process.GetProcessesByName("Cursor"))
        {
            try
            {
                var cible = Lire(processus);
                if (cible is not null)
                {
                    resultats.Add(cible);
                }
            }
            finally
            {
                processus.Dispose();
            }
        }

        return resultats;
    }

    public CursorTarget? TrouverPrincipal()
    {
        return Lister()
            .Where(c => c.EstCheminFiable && c.Fenetres.Count > 0)
            .OrderByDescending(c => c.Fenetres.Count)
            .FirstOrDefault();
    }

    private static CursorTarget? Lire(Process processus)
    {
        if (!CursorPathRules.EstNomProcessusCursor(processus.ProcessName))
        {
            return null;
        }

        string chemin;
        try
        {
            chemin = processus.MainModule?.FileName ?? string.Empty;
        }
        catch (Exception)
        {
            return null;
        }

        var fiable = CursorPathRules.EstCheminFiable(chemin);
        var version = string.Empty;
        try
        {
            version = FileVersionInfo.GetVersionInfo(chemin).FileVersion ?? string.Empty;
        }
        catch (Exception)
        {
            version = string.Empty;
        }

        return new CursorTarget(
            processus.Id,
            chemin,
            version,
            fiable,
            ListerFenetres(processus.Id));
    }

    private static IReadOnlyList<CursorWindowInfo> ListerFenetres(int processId)
    {
        var fenetres = new List<CursorWindowInfo>();
        var racine = AutomationElement.RootElement;
        var condition = new PropertyCondition(AutomationElement.ProcessIdProperty, processId);
        AutomationElementCollection trouves;
        try
        {
            trouves = racine.FindAll(TreeScope.Children, condition);
        }
        catch (Exception)
        {
            return fenetres;
        }

        foreach (AutomationElement element in trouves)
        {
            try
            {
                var r = element.Current.BoundingRectangle;
                if (r.Width < 80 || r.Height < 80)
                {
                    continue;
                }

                fenetres.Add(new CursorWindowInfo(
                    element.Current.Name ?? string.Empty,
                    element.Current.ClassName ?? string.Empty,
                    (int)r.X,
                    (int)r.Y,
                    (int)r.Width,
                    (int)r.Height));
            }
            catch (ElementNotAvailableException)
            {
            }
        }

        return fenetres;
    }
}
