using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace App1.ViewModels;

public class MainViewModel : ViewModelBase, IViewHost
{
    private ViewModelBase _rightView;
    public ViewModelBase RightView
    {
        get => _rightView;
        set => SetProperty(ref _rightView, value);
    }
    private ViewModelBase _leftView;
    public ViewModelBase LeftView
    {
        get => _leftView;
        set => SetProperty(ref _leftView, value);
    }
    private Store _store { get; } = new();

    public MainViewModel()
    {
        _ = InitializeAsync();
        _rightView = new WellcomeViewModel("Wellcome");
        _leftView = new GroupListViewModel(this, _store);
    }
    
        public async Task InitializeAsync()
    {
        _store.GroupedList = await TaskHelpers.LoadAsync();
        HookSaveOnIsDoneChange(_store.GroupedList);
    }

    private void HookSaveOnIsDoneChange(ObservableCollection<GroupList> groupedList)
    {
        foreach (var list in groupedList)
            foreach (var group in list.Groups)
                foreach (var task in group.Tasks)
                    TaskHelpers.HookSaveToTask(_store, task);
    }

    Task IViewHost.NavigateLeft(ViewModelBase viewModel)
    {
        LeftView = viewModel;
        return Task.CompletedTask;
    }

    Task IViewHost.NavigateRight(ViewModelBase viewModel)
    {
        RightView = viewModel;
        return Task.CompletedTask;
    }
}
