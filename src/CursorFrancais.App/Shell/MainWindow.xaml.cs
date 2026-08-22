using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CursorFrancais.App.Services;
using CursorFrancais.App.ViewModels;
using CursorFrancais.Core;

namespace CursorFrancais.App.Shell;

public partial class MainWindow : Window
{
    private readonly TranslatorViewModel _traducteur;
    private bool _quitterDemande;

    public MainWindow(ShellViewModel shell, TranslatorViewModel traducteur)
    {
        InitializeComponent();
        DataContext = shell;
        _traducteur = traducteur;
        NotifyIcon.IconSource = CreerIcone();
        Closing += OnClosing;
    }

    public void DemanderFermeture()
    {
        _quitterDemande = true;
        Close();
    }

    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_quitterDemande)
        {
            NotifyIcon.Dispose();
            return;
        }

        e.Cancel = true;
        Hide();
    }

    private void OnTrayOuvrir(object sender, RoutedEventArgs e)
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    private void OnTrayActiver(object sender, RoutedEventArgs e)
    {
        _traducteur.BasculerTraductionCommand.Execute(null);
    }

    private void OnTrayQuitter(object sender, RoutedEventArgs e)
    {
        _quitterDemande = true;
        Application.Current.Shutdown();
    }

    private static ImageSource CreerIcone()
    {
        const int taille = 16;
        var visuel = new DrawingVisual();
        using (var contexte = visuel.RenderOpen())
        {
            contexte.DrawRoundedRectangle(
                new SolidColorBrush(Color.FromRgb(61, 139, 255)),
                null,
                new Rect(0, 0, taille, taille),
                3,
                3);
            var texte = new FormattedText(
                "F",
                System.Globalization.CultureInfo.GetCultureInfo("fr-FR"),
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                11,
                Brushes.White,
                96);
            contexte.DrawText(texte, new Point(4, 0));
        }

        var bitmap = new RenderTargetBitmap(taille, taille, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(visuel);
        bitmap.Freeze();
        return bitmap;
    }
}
