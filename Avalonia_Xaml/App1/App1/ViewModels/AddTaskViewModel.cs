using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public partial class AddTaskViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    private readonly IDialogHelper _dialogHelper;
    private ViewModelBase _viewModel;
    public ICommand SaveTaskCommand { get; }
    public ICommand CancelCommand { get; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }

    public AddTaskViewModel(IViewHost host, ViewModelBase viewModel,  Store store)
    {
        _store = store;
        _host = host;
        _dialogHelper = new DialogHelper();
        _viewModel = viewModel;
        SaveTaskCommand = new AsyncRelayCommand(AddTask);
        CancelCommand = new AsyncRelayCommand(ClearAsync);
    }

    private async Task ClearAsync()
    {
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
        await _host.NavigateLeft(new GroupListViewModel(_host,_store));
        await _host.NavigateRight(_viewModel);
    }

    private async Task AddTask()
    {
        string name = TaskHelpers.InputOrDefault(NewTaskName, "");
        if (name == "")
        {
            bool? confirmed = await _dialogHelper.ShowDialogAsync("Name cannot be Empty");
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
        await TaskHelpers.AddTaskToCategory(task, _store);
        await ClearAsync();
    }
}
