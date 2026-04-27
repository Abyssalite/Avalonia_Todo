using System.Threading.Tasks;
using Avalonia_EventHub;
using Avalonia_Navigation;

namespace App1.ViewModels;

public partial class TaskDetailViewModel : ViewModelBase, IHandleBackNavigation
{
    public BaseTask? TaskItem { get; }
    public bool IsNotInArchive { get; } = true;
    private bool? _isDone = false;
    public bool? IsDone
    {
        get => _isDone;
        set
        {
            if (TaskItem == null) return;

            _isDone = value;
            _store.SetTaskDone(_isDone);
            OnPropertyChanged(nameof(IsDone));         
        }
    }

    public TaskDetailViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate,
        IEventHub events
    ): base(store, navigator, dialogService, stateService, notificate, events)
    {
        if (_store.SelectedTask == null || _store.SelectedList == null) return;
        
        TaskItem = _store.SelectedTask;
        IsNotInArchive = !_store.SelectedList.IsArchived;
        _isDone = TaskItem.IsDone;
    }
    
    protected override async Task DeleteAsync()
    {
        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true && TaskItem != null)
        {
            await _store.StoreDeleteTaskAsync(TaskItem, IsNotInArchive);
            await _navigator.OpenPrevious();
        }
    }

    async Task<bool> IHandleBackNavigation.HandleBackAsync()
    {
        _store.SelectTask(null);
        return await Task.FromResult(false);
    }
}
