using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace App1.ViewModels;

public partial class TaskGroupViewModel : ViewModelBase
{
    private readonly Store _store;
    public readonly INavigatorService _navigator;
    private readonly IDialogService _dialogService;
    private readonly IChangeStateService _stateService;
    public ObservableCollection<TaskGroup>? GroupedTasks { get; set; } = new();
    public string? ListName { get; set; }
    public string? QuickAddTaskName { get; set; }
    private string _oldListName = "";
    public bool IsNotMainList { get; } = true;
    public bool IsNotInArchive { get; set; } = true;
    public bool CanEditListName { get; set; } = false;
    private bool _isInEditMode = false;
    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            _isInEditMode = value;
            _stateService.IsInEditMode = value;
            CanEditListName = !CheckMainList() && _isInEditMode;
            OnPropertyChanged(nameof(IsInEditMode));
            OnPropertyChanged(nameof(CanEditListName));
        }
    }
    public ICommand AddTaskViewCommand { get; }
    public ICommand CancelCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public AsyncRelayCommand ArchiveCommand { get; }
    public AsyncRelayCommand RestoreOrEditCommand { get; }

    private BaseTask? _selectedTask;
    public BaseTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (value != null && value != _selectedTask)
            {
                _selectedTask = value;
                _ = OpenTaskAsync(_selectedTask);
                _selectedTask = null;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
    }

    public TaskGroupViewModel(Store store, INavigatorService navigator, IDialogService dialogService, IChangeStateService stateService)
    {
        _store = store;
        _navigator = navigator;
        _dialogService = dialogService;
        _stateService = stateService;
        if (store.SelectedList != null)
        {
            ListName = store.SelectedListName;
            GroupedTasks = store.SelectedList.Groups;
            IsNotMainList = !CheckMainList();
            IsNotInArchive = !store.SelectedList.IsArchived;
            OnPropertyChanged(nameof(IsNotInArchive));
            OnPropertyChanged(nameof(IsNotMainList));
        }

        DeleteCommand = new AsyncRelayCommand(DeleteListAsync);
        ArchiveCommand = new AsyncRelayCommand(ArchiveListAsync);
        RestoreOrEditCommand = new AsyncRelayCommand(RestoreOrEditListAsync);

        _stateService.EditModeChanged += (_, value) =>
        {
            IsInEditMode = value;
        };
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
        CancelCommand = new RelayCommand(CancelEdit);
    }

    private bool CheckMainList() => ListName == "Quick" || ListName == "Important";

    private async Task ArchiveListAsync()
    {
        if (!IsNotMainList || ListName == null) return;

        await TaskHelpers.MoveToArchive(ListName, _store);
        IsInEditMode = false;
    }

    private async Task DeleteListAsync()
    {
        if (!IsNotMainList || ListName == null) return;

        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true)
        {
            await TaskHelpers.DeleteList(ListName, _store, !IsNotInArchive);
            await _navigator.OpenPrevious();
        }        
        IsInEditMode = false;
    }

    private async Task RestoreOrEditListAsync()
    {
        if (ListName == null) return;

        if (IsNotInArchive && !_isInEditMode)
        {
            _oldListName = ListName;
            IsInEditMode = true;
        }
        else await TaskHelpers.MoveToList(ListName, _store);
    }

    private void CancelEdit()
    {
        if (_isInEditMode)
        {
            ListName = _oldListName;
            OnPropertyChanged(nameof(ListName));
            IsInEditMode = false;
        }
    }

    private async Task OpenAddOrSaveTaskAsync()
    {
        if (ListName == null || ListName == "") return;
        if (_isInEditMode && _oldListName != "")
            await TaskHelpers.EditList(_oldListName, ListName, GroupedTasks, _store);

        else
        {
            if (QuickAddTaskName != null && QuickAddTaskName != "")
            {
                var task = new BaseTask
                {
                    Name = QuickAddTaskName,
                    Category = "Miscelanious",
                    ListName = ListName,
                    Description = ""
                };
                QuickAddTaskName = null;
                OnPropertyChanged(nameof(QuickAddTaskName));
                await TaskHelpers.AddTaskToCategory(task, _store);
            }
            else
            {
                var vm = App.Services?.GetRequiredService<AddTaskViewModel>();
                await _navigator.NavigateRight(vm, new Components.TopBarViewModel(_store, vm, "New Task"));
                await _navigator.NavigateLeft(App.Services?.GetRequiredService<NewTaskOptionViewModel>());
            }
        }
        IsInEditMode = false;
    }

    private async Task OpenTaskAsync(BaseTask task)
    {
        _store.SelectedTask = task;
        var vm = App.Services?.GetRequiredService<TaskDetailViewModel>();
        await _navigator.NavigateRight(vm, new Components.TopBarViewModel(_store, vm));
    }
}
