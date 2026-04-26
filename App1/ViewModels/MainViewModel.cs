using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Avalonia_Navigation;
using Avalonia_EventHub;

namespace App1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IViewHost _host;
    public IViewHost ViewHost => _host;

    public MainViewModel(
        IViewHost host,
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate,
        IEventHub events
    ): base(store, navigator, dialogService, stateService, notificate, events)
    {
        _host = host;
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        _store.MainLists.MainLists = await TaskHelpers.LoadAsync("Tasks")  ?? _store.CreateDefaultList();
        _store.ArchiveLists.ArchivedLists = await TaskHelpers.LoadAsync("Archive") ?? [];

        _store.StoreUpdateImportantList();
        _store.StoreUpdateFilteredLists();

        var vm = App.Services?.GetRequiredService<WelcomeViewModel>();
        _navigator.FirstView = new NavigationState(
            vm,
            App.Services?.GetRequiredService<GroupListViewModel>(),
            new Components.TopBarViewModel(_store, vm, _events)
        );
        await _navigator.Navigate(_navigator.FirstView);
    }
}
