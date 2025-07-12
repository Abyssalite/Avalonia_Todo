using System.Threading.Tasks;

namespace App1.ViewModels;

public partial class TaskDetailViewModel : ViewModelBase
{
    public BaseTask? Task { get; }
    public bool IsNotInArchive { get; } = true;

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
        }
    }

    protected override async Task BackOrToggleDrawerAsync() =>
        await _navigator.OpenPrevious();
    
    protected override async Task DeleteListAsync()
    {
        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true && Task != null)
        {
            await TaskHelpers.DeleteTask(Task, _store, !IsNotInArchive);
            await _navigator.OpenPrevious();
        }
    }
}
