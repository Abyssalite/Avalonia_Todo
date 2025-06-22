using System.Text.Json;
using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class TaskDetailViewModel : ViewModelBase
{
    public BaseTask? Task { get; }
    private Store _store;
    private INavigator _navigator;
    public TaskDetailViewModel(Store store, INavigator nav)
    {
        _navigator = nav;
        _store = store;

        if (store.SelectedTask != null)
            Task = store.SelectedTask;
    }
}
