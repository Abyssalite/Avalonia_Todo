using Avalonia.Controls;
using Avalonia.Interactivity;

namespace App2.Views;

public partial class CustomDialog : Window
{
    public CustomDialog()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
