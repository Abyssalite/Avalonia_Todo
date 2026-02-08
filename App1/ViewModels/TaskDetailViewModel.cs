using System.Threading.Tasks;

namespace App1.ViewModels;

public partial class TaskDetailViewModel : ViewModelBase
{
    public BaseTask? Task { get; }
    public bool IsNotInArchive { get; } = true;
    private bool _isDone = false;
    public bool IsDone
    {
        get => _isDone;
        set
        {
            if (Task != null)
            {
                _isDone = value;
                Task.IsDone = _isDone;
                OnPropertyChanged(nameof(IsDone));         
            }
        }
    }

    public TaskDetailViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate) :
        base(store, navigator, dialogService, stateService, notificate)
    {
        if (store.SelectedTask != null && store.SelectedList != null)
        {
            Task = store.SelectedTask;
            IsNotInArchive = !store.SelectedList.IsArchived;
            _isDone = Task.IsDone;
        }
    }

    public override bool? GetSetImportant(bool? value)
    {
        if (Task == null) return null;
        if (value == null) return Task.IsImportant;
        else if (value == true)
        {
            Task.IsImportant = true;
        }
        else if (value == false)
        {
            Task.IsImportant = false;
        } 
        return null;
    }

    protected override async Task BackOrToggleDrawerAsync() =>
        await _navigator.OpenPrevious();
    
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
