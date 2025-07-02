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
    public ObservableCollection<TaskGroup>? GroupedTasks { get; set; } = new();
    public string? ListName { get; set; }
    private string OldListName = "";
    public bool IsNotMainList { get; } = true;
    public bool IsNotInArchive { get; set; } = true;
    private bool _isInEditMode = false;
    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            _isInEditMode = value;
            OnPropertyChanged(nameof(IsInEditMode));
        }
    }
    public ICommand AddTaskViewCommand { get; }
    public ICommand ArchiveOrDeleteCommand { get; }
    public ICommand RestoreOrEditCommand { get; }
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
            IsNotInArchive = !store.SelectedList.IsArchived;
            OnPropertyChanged(nameof(IsNotMainList));
        }
        _store.PropertyChanged += (_, e) =>
        {
            if (store.SelectedList == null) return;

            if (e.PropertyName == nameof(GroupList.IsArchived))
            {
                IsNotInArchive = !store.SelectedList.IsArchived;
                OnPropertyChanged(nameof(IsNotInArchive));
            }
            if (e.PropertyName == nameof(Store.FilteredLists))
            {
                GroupedTasks = store.SelectedList.Groups;
                OnPropertyChanged(nameof(ListName));
                OnPropertyChanged(nameof(GroupedTasks));
            }
        };

        AddTaskViewCommand = new AsyncRelayCommand(OpenAddOrSaveTaskAsync);
        ArchiveOrDeleteCommand = new AsyncRelayCommand<object>(ArchiveOrDeleteListAsync);
        RestoreOrEditCommand = new AsyncRelayCommand<object>(RestoreOrEditListAsync);
    }

    private bool CheckMainList() => ListName == "Quick" || ListName == "Important";

    private async Task ArchiveOrDeleteListAsync(object? parameter)
    {
        if (parameter is Button button)
            button.Flyout?.Hide();
        if (!IsNotMainList || ListName == null) return;

        if (IsNotInArchive)
            await TaskHelpers.MoveToArchive(ListName, _store);
        else await OnShowDialogAsync(ListName);
        IsInEditMode = false;
    }

    private async Task RestoreOrEditListAsync(object? parameter)
    {
        if (parameter is Button button)
            button.Flyout?.Hide();
        if (ListName == null) return;

        if (IsNotInArchive)
        {
            OldListName = ListName;
            IsInEditMode = !IsInEditMode;
        }
        else await TaskHelpers.MoveToList(ListName, _store);
    }

    private async Task OpenAddOrSaveTaskAsync()
    {
        if (ListName == null) return;
        if (IsInEditMode) await TaskHelpers.EditList(OldListName, ListName, GroupedTasks, _store);
        
        else
        {
            await _host.NavigateRight(App.Services?.GetRequiredService<AddTaskViewModel>());
            await _host.NavigateLeft(App.Services?.GetRequiredService<NewTaskOptionViewModel>());
        }
        IsInEditMode = false;
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
        if (confirmed == true)
        { 
            await TaskHelpers.DeleteList(listName, _store, true);
            await _host.NavigateRight(App.Services?.GetRequiredService<WellcomeViewModel>());
        }
    }
}
