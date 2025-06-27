using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;

namespace App1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public WindowNotificationManager? NotificationManager { get; set; }
    private Store _store;
    private IViewHost _host;
    public IViewHost ViewHost => _host;

    public MainViewModel(Store store, IViewHost host)
    {
        _store = store;
        _host = host;
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        _store.GroupedList = await TaskHelpers.LoadAsync();
        HookSaveOnIsDoneChange(_store.GroupedList);

        await _host.NavigateLeft(App.Services?.GetRequiredService<GroupListViewModel>());
        await _host.NavigateRight(App.Services?.GetRequiredService<WellcomeViewModel>());
    }

    private void HookSaveOnIsDoneChange(ObservableCollection<GroupList> groupedList)
    {
        foreach (var list in groupedList)
            foreach (var group in list.Groups)
                foreach (var task in group.Tasks)
                    TaskHelpers.HookSaveToTask(_store, task);
    }
}
