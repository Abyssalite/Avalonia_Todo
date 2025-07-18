using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace App1.ViewModels;

public partial class TaskGroupViewModel : ViewModelBase
{
    private ObservableCollection<TaskGroup>? _groupedTasks = new();
    public ObservableCollection<TaskGroup>? GroupedTasks
    {
        get => _groupedTasks;
        set
        {
            if (value != null)
            {
                _groupedTasks = value;
                OnPropertyChanged(nameof(GroupedTasks));
            }
        }
    }
    private ObservableCollection<TaskGroup>? _clone;
    public string ListName { get; set; } = "";
    public string? QuickAddTaskName { get; set; }
    public bool IsNotMainList { get; } = true;
    public bool IsNotInArchive { get; set; } = true;
    private bool _isInEditMode = false;
    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            _isInEditMode = value;
            _stateService.IsInEditMode = value;
            OnChangeListName?.Invoke(_isInEditMode && IsNotMainList);
            OnPropertyChanged(nameof(IsInEditMode));
        }
    }
    private BaseTask? _selectedTask;
    public BaseTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (value != null && _selectedTask != value)
            {
                _selectedTask = value;
                _ = OpenTaskAsync(_selectedTask);
                _selectedTask = null;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
    }
    public ICommand AddOrSaveTaskCommand { get; }
    public ICommand CancelCommand { get; }
    
    public TaskGroupViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate) :
        base(store, navigator, dialogService, stateService, notificate)
    {
        ListName = store.SelectedListName;
        IsNotMainList = !TaskHelpers.IsMainList(ListName);
        OnPropertyChanged(nameof(IsNotMainList));

        if (store.SelectedList != null)
        {
            GroupedTasks = store.SelectedList.Groups;
            IsNotInArchive = !store.SelectedList.IsArchived;
            OnPropertyChanged(nameof(IsNotInArchive));
        }

        _stateService.EditModeChanged += CancelEdit;
        _store.PropertyChanged += (_, e) =>
        {
            if (store.SelectedList == null) return;

            // Update GUI after toggle IsArchived
            if (e.PropertyName == nameof(GroupList.IsArchived))
            {
                IsNotInArchive = !store.SelectedList.IsArchived;
                OnPropertyChanged(nameof(IsNotInArchive));
            }
            // Update GUI after edit List Category
            if (e.PropertyName == nameof(Store.FilteredLists))
            {
                GroupedTasks = store.SelectedList.Groups;
            }
        };

        AddOrSaveTaskCommand = new AsyncRelayCommand(OpenAddOrSaveTaskAsync);
        CancelCommand = new RelayCommand(CancelEdit);
    }

    protected override async Task ToggleArchiveListAsync()
    {
        if (!IsNotMainList || ListName == "") return;
        IsInEditMode = false;

        if (IsNotInArchive) await TaskHelpers.MoveToArchive(ListName, _store);
        else await TaskHelpers.MoveToList(ListName, _store);
    }

    protected override async Task DeleteAsync()
    {
        if (!IsNotMainList || ListName == "") return;
        IsInEditMode = false;

        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true)
        {
            await TaskHelpers.DeleteList(ListName, _store, !IsNotInArchive);
            await _navigator.OpenPrevious();
        }        
    }

    protected override async Task BackOrToggleDrawerAsync()
    {
        _stateService.OpenPane(!_stateService.IsPaneOpen);
        await Task.CompletedTask;
    }
    
    protected override void Edit()
    {
        if (ListName == "" || GroupedTasks == null) return;

        if (IsNotInArchive && !_isInEditMode)
        {
            _clone = TaskHelpers.Clone(GroupedTasks);
            IsInEditMode = true;
        }
    }

    private void CancelEdit()
    {
        if (_isInEditMode && _clone != null && GroupedTasks != null)
        {
            IsInEditMode = false;
            GroupedTasks.Clear();
            foreach (var item in _clone)
            {
                GroupedTasks.Add(item);
            }
            _clone = null;
            _store.TopbarText = ListName;
        }
    }

    private async Task OpenAddOrSaveTaskAsync()
    {
        if (ListName == "" || (_store.TopbarText == "" && _isInEditMode)) return;

        if (_isInEditMode)
        {
            await TaskHelpers.EditList(ListName, _store.TopbarText, GroupedTasks, _store);
            ListName = _store.TopbarText;
            _store.SelectedListName = ListName;
        }
        else
        {
            if (QuickAddTaskName != null && QuickAddTaskName != "")
            {
                var task = new BaseTask
                {
                    Name = QuickAddTaskName,
                    IsDone = false,
                    Category = "Miscelanious",
                    ListName = (ListName == GlobalVariables.Important) ? GlobalVariables.Quick : ListName,
                    Description = "",
                    IsImportant = ListName == GlobalVariables.Important
                };
                QuickAddTaskName = null;
                OnPropertyChanged(nameof(QuickAddTaskName));
                await TaskHelpers.AddTaskToCategory(task, _store);
            }
            else
            {
                var vm = App.Services?.GetRequiredService<AddTaskViewModel>();
                await _navigator.NavigateRight((vm, new Components.TopBarViewModel(_store, vm, "New Task")));
                await _navigator.NavigateLeft(App.Services?.GetRequiredService<NewTaskOptionViewModel>());
            }
        }
        if (ListName == GlobalVariables.Important)
            GroupedTasks = TaskHelpers.FilterImportant(_store.Lists);
        IsInEditMode = false;
    }

    private async Task OpenTaskAsync(BaseTask task)
    {
        _store.SelectedTask = task;
        var vm = App.Services?.GetRequiredService<TaskDetailViewModel>();
        await _navigator.NavigateRight((vm, new Components.TopBarViewModel(_store, vm, ListName)));
    }
}
