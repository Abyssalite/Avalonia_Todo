using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace App1.ViewModels;

public partial class TaskGroupViewModel : ViewModelBase
{
    private readonly Store _store;
    private readonly IViewHost _host;
    private readonly IDialogService _dialogService;
    public ObservableCollection<TaskGroup>? GroupedTasks { get; }
    public string? ListName { get; }
    public ICommand AddTaskViewCommand { get; }
    public ICommand ShowDialogCommand { get; }
    private BaseTask? _selectedTask;
    public BaseTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (value != null && value != _selectedTask)
            {
                _selectedTask = value;
                _ = OpenTaskAsync(value);
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
    }

    public TaskGroupViewModel(IViewHost host, Store store, IDialogService dialogService)
    {
        _store = store;
        _host = host;
        _dialogService = dialogService;
        if (store.SelectedList != null)
        {
            ListName = store.ListName;
            GroupedTasks = store.SelectedList.Groups;
        }

        AddTaskViewCommand = new AsyncRelayCommand(OpenAddTaskView);
        ShowDialogCommand = new AsyncRelayCommand<object>(OnShowDialogAsync);
    }

    private async Task DeleteList()
    {
        if (ListName != null)
        {
            await TaskHelpers.DeleteList(ListName, _store);
            await _host.NavigateRight(App.Services?.GetRequiredService<WellcomeViewModel>());
        }
    }

    private async Task OpenAddTaskView()
    {
        if (ListName != null)
        {
            await _host.NavigateRight(App.Services?.GetRequiredService<AddTaskViewModel>());
            await _host.NavigateLeft(App.Services?.GetRequiredService<NewTaskOptionViewModel>());
        }
    }

    private async Task OpenTaskAsync(BaseTask task)
    {
        _selectedTask = null;
        OnPropertyChanged(nameof(SelectedTask));

        _store.SelectedTask = task;
        await _host.NavigateRight(App.Services?.GetRequiredService<TaskDetailViewModel>());

    }

    private async Task OnShowDialogAsync(object? parameter)
    {
        if (parameter is Button button)
            button.Flyout?.Hide();

        if (ListName == "Quick") return;

        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true) await DeleteList();
    }
}
