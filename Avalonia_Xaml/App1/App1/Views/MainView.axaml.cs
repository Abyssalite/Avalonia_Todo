using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Semi.Avalonia;
using CommunityToolkit.Mvvm.Input;

namespace App1.Views;

public partial class MainView : UserControl
{
    private bool _isPaneOpen;
    private bool _isSetted = false;
    private bool _isOverride = false;

    public MainView()
    {
        InitializeComponent();
        if (OperatingSystem.IsAndroid())
        {
            MainSplitView.Margin = new Thickness(0);
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            ContentGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
            Grid.SetRow(MainContent, 1);
            LeftBar.Orientation = Avalonia.Layout.Orientation.Horizontal;
            OpenPane(false);
            MainSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
        }
        else
        {
            MainSplitView.Margin = new Thickness(0,30,0,0);
            ContentGrid.ColumnDefinitions.Clear();
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            Grid.SetColumn(MainContent, 1);
            LeftBar.Orientation = Avalonia.Layout.Orientation.Vertical;
            OpenPane(true);
            MainSplitView.DisplayMode = SplitViewDisplayMode.Inline;
        }
        this.LayoutUpdated += (_, __) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var size = Convert.ToDouble(topLevel?.Bounds.Width);
            if (size > 550) _isOverride = false;
            if (!_isOverride)
            {
                if (_isPaneOpen && size < 600)
                {
                    _isSetted = true;
                    OpenPane(false);
                }
                else if (_isSetted && size > 650)
                {
                    _isSetted = false;
                    OpenPane(true);
                }
            }
        };
    }

    [RelayCommand]
    private void FollowSystemTheme()
    {
        Application.Current?.RegisterFollowSystemTheme();
    }

    private void MainContent_Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (OperatingSystem.IsAndroid())
        {
            _isPaneOpen = false;
            _isSetted = true;
        }
    }

    private void OpenPane(bool value)
    {
        _isPaneOpen = value;
        MainSplitView.IsPaneOpen = _isPaneOpen;
        if (OperatingSystem.IsAndroid()) return;
        LeftBar.IsVisible = !_isPaneOpen;
    }

    private void TogglePane_Click(object sender, RoutedEventArgs e)
    {
        _isOverride = true;
        OpenPane(!_isPaneOpen);
    }
}