using System.IO;
using System.Text.Json;
using System.Windows.Automation;
using CursorFrancais.Automation;
using CursorFrancais.Native;

var locator = new CursorLocator();
var cibles = locator.Lister().Where(c => c.EstCheminFiable).ToList();
if (cibles.Count == 0)
{
    Console.Error.WriteLine("Aucun Cursor fiable détecté. Ouvre Cursor puis relance DumpCursorUi.");
    return 2;
}

var lecteur = new UiaReader();
var racineUia = AutomationElement.RootElement;
var sorties = new List<object>();

foreach (var cible in cibles)
{
    var condition = new PropertyCondition(AutomationElement.ProcessIdProperty, cible.ProcessId);
    AutomationElementCollection fenetres;
    try
    {
        fenetres = racineUia.FindAll(TreeScope.Children, condition);
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine("Lecture UIA impossible : " + exception.Message);
        continue;
    }

    foreach (AutomationElement fenetre in fenetres)
    {
        try
        {
            var hwnd = new nint(fenetre.Current.NativeWindowHandle);
            var dpi = WindowDpi.Lire(hwnd);
            var rect = fenetre.Current.BoundingRectangle;
            var elements = lecteur.LireVisibles(fenetre);
            sorties.Add(new
            {
                titre = fenetre.Current.Name,
                classe = fenetre.Current.ClassName,
                processId = cible.ProcessId,
                exe = cible.ExePath,
                version = cible.VersionFichier,
                dpi,
                echellePourcent = Math.Round(dpi / 96.0 * 100),
                gauche = (int)rect.X,
                haut = (int)rect.Y,
                largeur = (int)rect.Width,
                hauteur = (int)rect.Height,
                nombreElements = elements.Count,
                elements,
            });
        }
        catch (ElementNotAvailableException)
        {
        }
    }
}

if (sorties.Count == 0)
{
    Console.Error.WriteLine("Cursor est lancé mais aucune fenêtre UIA n'a été lue.");
    return 3;
}

var dossier = ArgsDossier(args);
Directory.CreateDirectory(dossier);
var nom = $"dump-{DateTime.Now:yyyyMMdd-HHmmss}.json";
var chemin = Path.Combine(dossier, nom);
var json = JsonSerializer.Serialize(
    new { horodatage = DateTime.Now.ToString("o"), fenetres = sorties },
    new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(chemin, json);
Console.WriteLine(chemin);
return 0;

static string ArgsDossier(string[] arguments)
{
    if (arguments.Length > 0 && !string.IsNullOrWhiteSpace(arguments[0]))
    {
        return arguments[0];
    }

    return Path.Combine(AppContext.BaseDirectory, "dumps");
}
