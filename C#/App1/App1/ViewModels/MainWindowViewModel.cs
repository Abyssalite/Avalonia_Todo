namespace App1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private object _mainViewModel;
    public object MainView
    {
        get => _mainViewModel;
        set => SetProperty(ref _mainViewModel, value);
    }

    public MainWindowViewModel()
    {
        _mainViewModel = new MainViewModel(this);
    }
}