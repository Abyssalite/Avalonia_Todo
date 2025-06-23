namespace AppUno.Views;

public sealed partial class MainView : Page
{
    private bool _isPaneOpen = true;
    
    public MainView()
    {
        this.InitializeComponent();
#if __ANDROID__
        var permissions = new PermissionsService();
        _ = permissions.RequestStoragePermissionsAsync();
#endif

        LeftPane.IsPaneOpen = _isPaneOpen;
        LeftBar.Visibility = Visibility.Collapsed;

    }

    private void TogglePane_Click(object sender, RoutedEventArgs e)
    {
        _isPaneOpen = !_isPaneOpen;
        LeftPane.IsPaneOpen = _isPaneOpen;
        LeftBar.Visibility = _isPaneOpen ? Visibility.Collapsed : Visibility.Visible;
    }
}

