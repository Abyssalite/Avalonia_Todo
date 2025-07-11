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
using Avalonia.Media;

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
                LeftBar.IsVisible = !_isPaneOpen || OperatingSystem.IsAndroid();
            });

        if (OperatingSystem.IsAndroid())
        {
            var stateService = App.Services?.GetRequiredService<IChangeStateService>();
            if (stateService is ChangeStateService service) service.PaneChanged += OpenPane;

            TopBorder.Height = 0;
            OpenPane(false);
            MainSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
        }
        else
        {
            TopBorder.Height = OperatingSystem.IsBrowser() ? 0 : 33;
            OpenPane(true);
            MainSplitView.DisplayMode = SplitViewDisplayMode.Inline;
        }

        this.LayoutUpdated += (_, __) =>
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
        this.AttachedToVisualTree += (_, __) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var insetsManager = topLevel?.InsetsManager;

            if (insetsManager is not null)
            {
                insetsManager.SystemBarColor = Colors.Black;
                insetsManager.DisplayEdgeToEdge = true;
            }
        };
    }

    private void AdjustPaneLayout()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var width = topLevel.Bounds.Width;
        if (width > 650) _isOverride = false;

        if (!_isOverride)
        {
            if (_isPaneOpen && width < 650)
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