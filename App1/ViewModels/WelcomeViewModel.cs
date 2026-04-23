using System.Threading.Tasks;
using Avalonia_EventHub;
using Avalonia_Navigation;

namespace App1.ViewModels;

public partial class WelcomeViewModel : ViewModelBase
{
    public string Text { get; }
    public WelcomeViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate,
        IEventHub events
    ): base(store, navigator, dialogService, stateService, notificate, events)
    {
        Text = store.WelcomeText;
    }
}
