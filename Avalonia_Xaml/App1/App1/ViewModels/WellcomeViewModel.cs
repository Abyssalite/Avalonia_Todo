using System.Threading.Tasks;

namespace App1.ViewModels;

public partial class WellcomeViewModel : ViewModelBase
{
    public string Text { get; }
    public WellcomeViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate) :
        base(store, navigator, dialogService, stateService, notificate)
    {
        Text = store.WellcomeText;
    }
    
    protected override async Task BackOrToggleDrawerAsync()
    {
        _stateService.OpenPane(!_stateService.IsPaneOpen);
        await Task.CompletedTask;
    }
}
