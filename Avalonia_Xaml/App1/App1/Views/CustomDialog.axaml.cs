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

    private void OnYesClick(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }
        private void OnNoClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
