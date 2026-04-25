using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Avalonia_Navigation;
using Avalonia_EventHub;
using App1.Events;

namespace App1.ViewModels;

public partial class TaskGroupViewModel : ViewModelBase, IHandleBackNavigation
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
            _store.OnChangeListName(_isInEditMode);
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
    public ICommand? AddOrSaveTaskCommand { get; }
    public ICommand? CancelCommand { get; }
    
    public TaskGroupViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate,
        IEventHub events
    ): base(store, navigator, dialogService, stateService, notificate, events)
    {
        if (_store.SelectedList == null || _store.SelectedListName == null) return;

        ListName = _store.SelectedListName;
        IsNotMainList = !TaskHelpers.IsMainList(ListName);

        GroupedTasks = _store.SelectedList.Groups;
        IsNotInArchive = !_store.SelectedList.IsArchived;
        

        _stateService.CancelEditAction += CancelEdit;
        AddOrSaveTaskCommand = new AsyncRelayCommand(OpenAddOrSaveTaskAsync);
        CancelCommand = new RelayCommand(CancelEdit);


        _subscriptions.Add(_events.Subscribe<GroupListIsArchiveStateChangedEvent>(evt =>
        {
            if (evt.List.ListName != _store.SelectedList.ListName) return;

            IsNotInArchive = !evt.IsArchived;
            OnPropertyChanged(nameof(IsNotInArchive));
        }));


        _subscriptions.Add(_events.Subscribe<GroupListChangedEvent>(evt =>
        {
            if (evt.Groups != _store.SelectedList.Groups) return;

            GroupedTasks = evt.Groups;
        }));

        _subscriptions.Add(_events.Subscribe<ChangeImportantListEvent>(evt =>
        {
            GroupedTasks = _store.ImportantList;
        }));
    }

    protected override void ToggleArchiveList()
    {
        if (!IsNotMainList || ListName == "") return;
        IsInEditMode = false;

        if (IsNotInArchive) _store.StoreMoveToArchive(ListName);
        else _store.StoreMoveToList(ListName);
    }

    protected override async Task DeleteAsync()
    {
        if (!IsNotMainList || ListName == "") return;
        IsInEditMode = false;

        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true)
        {
            _store.StoreDeleteList(ListName, !IsNotInArchive);
            await _navigator.OpenPrevious();
        }        
    }

    protected override void Edit()
    {
        if (ListName == "" || GroupedTasks == null) return;

        if (IsNotInArchive && !_isInEditMode)
        {
            //_clone = StoreHelpers.Clone(GroupedTasks);
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
            var tmp = _store.TopbarText ?? "Miscelanious";
            _store.StoreEditList(ListName, tmp, GroupedTasks);
            ListName = tmp;
            //_store.SelectedListName = ListName;
        }
        else
        {
            if (QuickAddTaskName != null && QuickAddTaskName != "")
            {
                var task = new BaseTask(_events)
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
                _store.StoreAddTaskToCategory(task);
                _store.StoreUpdateImportantList();
            }
            else
            {
                var vm = App.Services?.GetRequiredService<AddTaskViewModel>();
                await _navigator.Navigate(new NavigationState(
                    vm, 
                    App.Services?.GetRequiredService<NewTaskOptionViewModel>(),
                    new Components.TopBarViewModel(_store, vm, _events, "New Task")
                ));

            }
        }
        IsInEditMode = false;
    }

    async Task<bool> IHandleBackNavigation.HandleBackAsync()
    {
        if (_isInEditMode)
        {
            IsInEditMode = false;
            return await Task.FromResult(true);
        }
        else return await Task.FromResult(false);
    }

    private async Task OpenTaskAsync(BaseTask task)
    {
        _store.SelectedTask = task;
        var vm = App.Services?.GetRequiredService<TaskDetailViewModel>();
        await _navigator.NavigateMainAndTop(vm, new Components.TopBarViewModel(_store, vm, _events, ListName));

    }
}
