using System.Threading.Tasks;
using App1.Events;
using Avalonia_EventHub;
using Avalonia_Navigation;

namespace App1.ViewModels;

public partial class TaskDetailViewModel : ViewModelBase
{
    public BaseTask? Task { get; }
    public bool IsNotInArchive { get; } = true;
    private bool? _isDone = false;
    public bool? IsDone
    {
        get => _isDone;
        set
        {
            if (Task != null)
            {
                _isDone = value;
                _store.SetTaskDone(_isDone);
                OnPropertyChanged(nameof(IsDone));         
            }
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
        
        Task = _store.SelectedTask;
        IsNotInArchive = !_store.SelectedList.IsArchived;
        _isDone = Task.IsDone;
    }
    
    protected override async Task DeleteAsync()
    {
        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true && Task != null)
        {
            await TaskHelpers.DeleteTask(Task, _store, !IsNotInArchive);
            await _navigator.OpenPrevious();
        }
    }
}
