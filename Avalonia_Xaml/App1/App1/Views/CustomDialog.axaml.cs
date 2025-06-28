using Avalonia.Controls;
using Avalonia.Interactivity;

namespace App1.Views;

public partial class CustomDialog : Window
{
    public CustomDialog(string message, bool isShow, string buttonName)
    {
        InitializeComponent();
        MessageText.Text = message;
        YesButton.Content = buttonName;
        NoButton.IsVisible = isShow;
    }

    private void OnYes_Click(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }
        private void OnNo_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
