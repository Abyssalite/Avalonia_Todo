using System.ComponentModel;
using System.Threading.Tasks;
using App1.Components;
using App1.ViewModels;

public class ViewHost : IViewHost, INotifyPropertyChanged
{
    private ViewModelBase _leftView = null!;
    private ViewModelBase _rightView = null!;
    private TopBarViewModel? _topBar = null!;

    public ViewModelBase LeftView
    {
        get => _leftView;
        set
        {
            if (_leftView != value)
            {
                _leftView = value;
                OnPropertyChanged(nameof(LeftView));
            }
        }
    }

    public ViewModelBase RightView
    {
        get => _rightView;
        set
        {
            if (_rightView != value)
            {
                _rightView = value;
                OnPropertyChanged(nameof(RightView));
            }
        }
    }

    public TopBarViewModel? TopBar
    {
        get => _topBar;
        set
        {
            if (_topBar != value)
            {
                _topBar = value;
                OnPropertyChanged(nameof(TopBar));
            }
        }
    }

    public Task NavigateLeft(ViewModelBase? viewModel)
    {
        if (viewModel != null)
            LeftView = viewModel;
        return Task.CompletedTask;
    }

    public Task NavigateRight(ViewModelBase? viewModel)
    {
        if (viewModel != null)
            RightView = viewModel;
        return Task.CompletedTask;
    }

    public Task ChangeTopBar(TopBarViewModel? viewModel)
    {
        TopBar = viewModel;
        return Task.CompletedTask;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public interface IViewHost
{
    ViewModelBase LeftView { get; set; }
    ViewModelBase RightView { get; set; }
    TopBarViewModel? TopBar { get; set; }

    Task ChangeTopBar(TopBarViewModel? viewModel);
    Task NavigateLeft(ViewModelBase? viewModel);
    Task NavigateRight(ViewModelBase? viewModel);
}

