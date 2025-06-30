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
    public bool IsNotMainList { get; } = true;
    public bool IsNotInArchive { get; set; } = true;
    public ICommand AddTaskViewCommand { get; }
    public ICommand ArchiveOrDeleteCommand { get; }
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
            ListName = store.SelectedListName;
            GroupedTasks = store.SelectedList.Groups;
            IsNotMainList = !CheckMainList();
            OnPropertyChanged(nameof(IsNotMainList));
            IsNotInArchive = !store.SelectedList.IsArchived;
        }
        _store.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(GroupList.IsArchived) && store.SelectedList != null)
            {
                IsNotInArchive = !store.SelectedList.IsArchived;
                OnPropertyChanged(nameof(IsNotInArchive));
            }
        };

        AddTaskViewCommand = new AsyncRelayCommand(OpenAddTaskView);
        ArchiveOrDeleteCommand = new AsyncRelayCommand<object>(ArchiveOrDeleteList);
    }

    private bool CheckMainList() => ListName == "Quick" || ListName == "Important";

    private async Task ArchiveOrDeleteList(object? parameter)
    {
        if (parameter is Button button)
            button.Flyout?.Hide();
        if (!IsNotMainList || ListName == null) return;

        if (IsNotInArchive)
            await TaskHelpers.MoveToArchive(ListName, _store);
        else
        {
            await OnShowDialogAsync(ListName);
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

    private async Task OnShowDialogAsync(string listName)
    {
        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true) await TaskHelpers.DeleteList(listName, _store, true);

    }
}
