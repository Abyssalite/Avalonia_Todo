using System.Collections.ObjectModel;
using System.Text.Json;
using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class TaskGroupViewModel : ViewModelBase
{
    public INavigator _navigator;
    private Store _store { get; }
    public ObservableCollection<TaskGroup>? GroupedTasks { get; }
    public string? ListName { get; }
    public ICommand? AddTaskViewCommand { get; }
    public ICommand OpenTaskCommand { get; }
    public TaskGroupViewModel(Store store, INavigator nav)
    {
        _navigator = nav;
        _store = store;
        if (store.SelectedList != null)
        {
            ListName = store.SelectedList.List;
            GroupedTasks = store.SelectedList.Groups;
        }

        OpenTaskCommand = new RelayCommand<BaseTask>(OpenTask);
    }

    private async void OpenTask(BaseTask? task)
    {
        if (task != null)
        {
            _store.SelectedTask = task;
            await _navigator.NavigateViewModelAsync<TaskDetailViewModel>(this, data: _store); 
        } 
    }
}
