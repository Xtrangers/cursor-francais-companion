using System.Windows;

namespace CursorFrancais.App.Views;

public partial class DisclaimerWindow : Window
{
    public bool Accepte { get; private set; }

    public DisclaimerWindow()
    {
        InitializeComponent();
    }

    private void OnAccepter(object sender, RoutedEventArgs e)
    {
        Accepte = true;
        DialogResult = true;
        Close();
    }

    private void OnRefuser(object sender, RoutedEventArgs e)
    {
        Accepte = false;
        DialogResult = false;
        Close();
    }
}
