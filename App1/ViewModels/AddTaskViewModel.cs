using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Avalonia_Navigation;
using Avalonia_EventHub;
using App1.Events;

namespace App1.ViewModels;

public partial class AddTaskViewModel : ViewModelBase
{
    public ICommand? SaveTaskCommand { get; }
    public ICommand? CancelCommand { get; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
    public bool? _isImportant;
    private string _listName = "";


    public AddTaskViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate,
        IEventHub events
    ): base(store, navigator, dialogService, stateService, notificate, events)
    {        
        if (_store.SelectedListName == null) return;

        SaveTaskCommand = new AsyncRelayCommand(AddTask);
        CancelCommand = new AsyncRelayCommand(ClearAsync);
        
        _listName = _store.SelectedListName;
        _isImportant = _listName == GlobalVariables.Important;

        _subscriptions.Add(_events.Subscribe<TaskIsImportantChangedEvent>(evt =>
        {
            _isImportant = evt.IsImportant;
        }));
    }

    private async Task ClearAsync()
    {
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
        await _navigator.OpenPrevious();
    }

    private async Task AddTask()
    {
        string name = TaskHelpers.InputOrDefault(NewTaskName, "");
        if (name == "")
        {
            Notificate.ShowNotification("Task name cannot be Empty!");
            return;
        }
        var task = new BaseTask(_events)
        {
            Name = name,
            IsDone = false,
            ListName = (_listName == GlobalVariables.Important) ? GlobalVariables.Quick : _listName,
            Category = TaskHelpers.InputOrDefault(TaskCatalog, "Miscelanious"),
            Description = TaskHelpers.InputOrDefault(TaskDesc, ""),
            IsImportant = _isImportant
        };
        await _store.StoreAddTaskToCategoryAsync(task, _listName);
        
        await ClearAsync();
    }
}
