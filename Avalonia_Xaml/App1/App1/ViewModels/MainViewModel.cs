using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace App1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Store _store;
    private readonly IViewHost _host;
    public IViewHost ViewHost => _host;
    public INotificationService Notificate  { get; set; }

    public MainViewModel(Store store, IViewHost host, INotificationService notificate)
    {
        _store = store;
        _host = host;
        Notificate = notificate;
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        _store.GroupedList = await TaskHelpers.LoadAsync();
        _store.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(BaseTask.IsDone)) await TaskHelpers.SaveAsync(_store.GroupedList);
            
        };

        await _host.NavigateLeft(App.Services?.GetRequiredService<GroupListViewModel>());
        await _host.NavigateRight(App.Services?.GetRequiredService<WellcomeViewModel>());
    }

}
