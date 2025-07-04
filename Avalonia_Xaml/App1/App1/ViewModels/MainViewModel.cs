using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace App1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Store _store;
    private readonly IViewHost _host;
    public IViewHost ViewHost => _host;
    public readonly INavigatorService _navigator;
    public INotificationService Notificate { get; set; }

    public MainViewModel(Store store, IViewHost host, INavigatorService navigator, INotificationService notificate)
    {
        _store = store;
        _host = host;
        Notificate = notificate;
        _navigator = navigator;
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        _store.Lists = await TaskHelpers.LoadAsync();
        _store.Archive.ArchivedLists = await TaskHelpers.LoadAsync(true);

        _store.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(BaseTask.IsDone)) await TaskHelpers.SaveAsync(_store);
        };

        _navigator.FirstView = App.Services?.GetRequiredService<WellcomeViewModel>();
        await _navigator.NavigateLeft(App.Services?.GetRequiredService<GroupListViewModel>());
        await _navigator.NavigateRight(_navigator.FirstView);
    }
}
