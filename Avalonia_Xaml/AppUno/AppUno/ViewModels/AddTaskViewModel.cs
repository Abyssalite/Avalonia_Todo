using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class AddTaskViewModel : ViewModelBase
{
    private Store _store;
    private INavigator _navigator;
    public BaseTask? Task { get; }
    public Action? ShowEmptyNameDialog { get; set; }
    public ICommand ClearCommand { get; }
    public ICommand SaveTaskCommand { get; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }

    public AddTaskViewModel(Store store, INavigator nav)
    {
        _navigator = nav;
        _store = store;
        if (store.SelectedTask != null)
            Task = store.SelectedTask;
            
        ClearCommand = new RelayCommand(Clear);
        SaveTaskCommand = new RelayCommand(AddTask);
    }

    private void AddTask()
    {
        string name = TaskHelpers.InputOrDefault(NewTaskName, "");
        if (name == "")
        {
            ShowEmptyNameDialog?.Invoke();
            return;
        }
        var task = new BaseTask
        {
            Name = name,
            IsDone = false,
            List = TaskHelpers.InputOrDefault(_store.ListName, "Quick"),
            Category = TaskHelpers.InputOrDefault(TaskCatalog, "Miscelanious"),
            Description = TaskHelpers.InputOrDefault(TaskDesc, "")
        };
        TaskHelpers.AddTaskToCategory(task, _store);
        Clear();
        _navigator.GoBack(this);
    }

    private void Clear()
    {
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
    }
}
