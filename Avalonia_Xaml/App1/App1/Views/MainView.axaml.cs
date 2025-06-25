using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace App1.Views;

public partial class MainView : UserControl
{
    private bool _isPaneOpen;

    public MainView()
    {
        InitializeComponent();
        if (OperatingSystem.IsAndroid())
        {
            _isPaneOpen = false;
            LeftPane.IsPaneOpen = _isPaneOpen;
            LeftBar.IsVisible = !_isPaneOpen;
        }
        else
        {
            _isPaneOpen = true;
            LeftPane.IsPaneOpen = _isPaneOpen;
            LeftBar.IsVisible = !_isPaneOpen;
        }

    }

    private void TogglePane_Click(object sender, RoutedEventArgs e)
    {
        _isPaneOpen = !_isPaneOpen;
        LeftPane.IsPaneOpen = _isPaneOpen;
        LeftBar.IsVisible = !_isPaneOpen;
    }

}