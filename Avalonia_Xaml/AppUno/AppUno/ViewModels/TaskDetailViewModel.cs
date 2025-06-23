using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class TaskDetailViewModel : ViewModelBase
{
    private Store _store;
    private INavigator _navigator;
    public BaseTask? Task { get; }
    public ICommand DeleteTaskCommand { get; }

    public TaskDetailViewModel(Store store, INavigator nav)
    {
        _navigator = nav;
        _store = store;
        if (store.SelectedTask != null)
            Task = store.SelectedTask;

        DeleteTaskCommand = new RelayCommand(DeleteTask);
    }

    private async void DeleteTask()
    {
        if (Task != null)
        {
            TaskHelpers.DeleteTask(Task, _store);
            await _navigator.GoBack(this);    
        }

    }
}
