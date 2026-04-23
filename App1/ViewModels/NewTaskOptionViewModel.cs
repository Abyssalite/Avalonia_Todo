namespace App1.ViewModels;

using Avalonia_EventHub;
using Avalonia_Navigation;

public partial class NewTaskOptionViewModel : ViewModelBase
{
    public NewTaskOptionViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate,
        IEventHub events
    ): base(store, navigator, dialogService, stateService, notificate, events)
    {
    }
}