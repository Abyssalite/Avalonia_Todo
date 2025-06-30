using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using Semi.Avalonia;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using Ursa.Controls;
using App1.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace App1.Views;

public partial class MainView : UserControl
{
    private MainViewModel? _viewModel;
    private bool _isPaneOpen;
    private bool _isSetted = false;
    private bool _isOverride = false;
    private CancellationTokenSource? _resizeToken;

    public MainView()
    {
        InitializeComponent();
        MainSplitView.GetObservable(SplitView.IsPaneOpenProperty)
        .Subscribe(isOpen =>
        {
            _isPaneOpen = isOpen;
            if (OperatingSystem.IsAndroid()) return;
                LeftBar.IsVisible = !_isPaneOpen;
        });

        if (OperatingSystem.IsAndroid())
        {
            var paneService = App.Services?.GetRequiredService<IPaneService>();
            if (paneService is PaneService service) service.PaneChanged += OpenPane;

            TopBorder.Height = 0;
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
            TopBorder.Height = OperatingSystem.IsBrowser() ? 0 : 33;
            ContentGrid.ColumnDefinitions.Clear();
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            Grid.SetColumn(MainContent, 1);
            LeftBar.Orientation = Avalonia.Layout.Orientation.Vertical;
            OpenPane(true);
            MainSplitView.DisplayMode = SplitViewDisplayMode.Inline;
        }

        this.LayoutUpdated += (_, _) =>
        {
            _resizeToken?.Cancel();
            _resizeToken = new CancellationTokenSource();

            var token = _resizeToken.Token;
            Task.Delay(50, token).ContinueWith(t =>
            {
                if (t.IsCanceled) return;
                Dispatcher.UIThread.InvokeAsync(AdjustPaneLayout);
            });
        };
    }

    private void AdjustPaneLayout()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var width = topLevel.Bounds.Width;
        if (width > 550) _isOverride = false;

        if (!_isOverride)
        {
            if (_isPaneOpen && width < 600)
            {
                _isSetted = true;
                OpenPane(false);
            }
            else if (_isSetted && width > 650)
            {
                _isSetted = false;
                OpenPane(true);
            }
        }
    }

    [RelayCommand]
    private void FollowSystemTheme() =>
        Application.Current?.RegisterFollowSystemTheme();
    
    private void OpenPane(bool value) =>
        MainSplitView.IsPaneOpen = value;

    private void TogglePane_Click(object sender, RoutedEventArgs e)
    {
        _isOverride = true;
        OpenPane(!_isPaneOpen);
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _viewModel = this.DataContext as MainViewModel;
        var visualLayerManager = this.FindAncestorOfType<VisualLayerManager>();
        if (_viewModel == null) return;
        _viewModel.Notificate.NotificationManager =
            WindowNotificationManager.TryGetNotificationManager(visualLayerManager, out var notificationManager)
                ? notificationManager
                : new WindowNotificationManager(visualLayerManager) { MaxItems = 3 };
        _viewModel.Notificate.ToastManager = WindowToastManager.TryGetToastManager(visualLayerManager, out var toastManager)
            ? toastManager
            : new WindowToastManager(visualLayerManager) { MaxItems = 3 };
        Debug.Assert(WindowNotificationManager.TryGetNotificationManager(visualLayerManager, out _));
    }
}