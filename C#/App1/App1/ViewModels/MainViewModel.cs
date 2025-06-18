namespace App1.ViewModels;

public class MainViewModel : ViewModelBase
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
        _store.GroupedList = TaskHelpers.Load();
        HookSaveOnIsDoneChange();
        _rightView = new WellcomeViewModel("Wellcome");
        _leftView = new GroupListViewModel(this, _store);
    }

    private void HookSaveOnIsDoneChange()
{
    foreach (var list in _store.GroupedList)
    foreach (var group in list.Groups)
    foreach (var task in group.Tasks)
        TaskHelpers.HookSaveToTask(_store, task);

}
}
