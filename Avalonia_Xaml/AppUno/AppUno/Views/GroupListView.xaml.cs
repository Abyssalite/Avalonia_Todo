namespace AppUno.Views;

public sealed partial class GroupListView : UserControl
{
    private bool _isTextBoxshow = false;

    public GroupListView()
    {
        this.InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;

        NewListBox.Visibility = Visibility.Collapsed;

    }

    private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (DataContext is GroupListViewModel vm)
        {
            vm.OnSaveAddList = () =>
            {
                NewListBox.Text = null;
                _isTextBoxshow = !_isTextBoxshow;
                NewListBox.Visibility = _isTextBoxshow ? Visibility.Collapsed : Visibility.Visible;
            };
        }
    }
}
