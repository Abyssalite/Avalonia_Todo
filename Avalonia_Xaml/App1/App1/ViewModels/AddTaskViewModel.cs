using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Ursa.Controls;

namespace App1.ViewModels;

public partial class AddTaskViewModel : ViewModelBase
{
    private Store _store;
    private IViewHost _host;
    private readonly IDialogService _dialogService;
    public ICommand SaveTaskCommand { get; }
    public ICommand CancelCommand { get; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }

    public AddTaskViewModel(IViewHost host, Store store,IDialogService dialogService)
    {
        _store = store;
        _host = host;
        _dialogService = dialogService;
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
        await _host.NavigateRight(App.Services?.GetRequiredService<TaskGroupViewModel>());
        await _host.NavigateLeft(App.Services?.GetRequiredService<GroupListViewModel>());
    }

    private async Task AddTask()
    {
        string name = TaskHelpers.InputOrDefault(NewTaskName, "");
        if (name == "")
        {
            _dialogService.ShowNotification("Name cannot be Empty", "TopCenter");
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
