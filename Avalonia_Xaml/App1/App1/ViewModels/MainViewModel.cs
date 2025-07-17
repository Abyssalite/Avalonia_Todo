using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
        INotificationService notificate):
        base(store, navigator, dialogService, stateService, notificate)

    {
        _host = host;
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        _store.Lists = await TaskHelpers.LoadAsync();
        _store.Archive.ArchivedLists = await TaskHelpers.LoadAsync(true);

        _store.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(BaseTask.IsDone) || 
                e.PropertyName == nameof(BaseTask.IsImportant))
                await TaskHelpers.SaveAsync(_store);
        };

        var vm = App.Services?.GetRequiredService<WelcomeViewModel>();
        _navigator.FirstView = (vm, new Components.TopBarViewModel(_store, vm, ""));
        await _navigator.NavigateLeft(App.Services?.GetRequiredService<GroupListViewModel>());
        await _navigator.NavigateRight(_navigator.FirstView);
    }
}
