using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public partial class MainViewModel : ViewModelBase
{
    private Store _store = new Store();
    public INavigator _navigator;

    public MainViewModel(INavigator nav)
    {
        _navigator = nav;
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        _store.GroupedList = await TaskHelpers.LoadAsync();
        await _navigator.NavigateViewModelAsync<GroupListViewModel>(this, "Pane/", data: _store);
        await _navigator.NavigateViewModelAsync<WellcomeViewModel>(this, "App/", data: "Wellcome");
        HookSaveOnIsDoneChange();
    }

    private void HookSaveOnIsDoneChange()
    {
        foreach (var list in _store.GroupedList)
            foreach (var group in list.Groups)
                foreach (var task in group.Tasks)
                    TaskHelpers.HookSaveToTask(_store, task);
    }
}
