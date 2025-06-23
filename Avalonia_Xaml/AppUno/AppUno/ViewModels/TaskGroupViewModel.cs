using System.Collections.ObjectModel;
using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class TaskGroupViewModel : ViewModelBase
{
    private Store _store { get; }
    public INavigator _navigator;
    public ObservableCollection<TaskGroup>? GroupedTasks { get; }
    public ICommand? AddTaskViewCommand { get; }
    public ICommand OpenTaskCommand { get; }
    public ICommand DeleteListCommand { get; }
    public string? ListName { get; }

    public TaskGroupViewModel(Store store, INavigator nav)
    {
        _navigator = nav;
        _store = store;
        if (store.SelectedList != null)
        {
            ListName = store.ListName;
            GroupedTasks = store.SelectedList.Groups;
        }

        OpenTaskCommand = new RelayCommand<BaseTask>(OpenTask);
        AddTaskViewCommand = new RelayCommand(OpenAddTaskView);
        DeleteListCommand = new RelayCommand(DeleteList);
    }

    private async void DeleteList()
    {
        if (ListName != null && ListName != "Quick")
        { 
            TaskHelpers.DeleteList(ListName, _store);
            await _navigator.NavigateViewModelAsync<WellcomeViewModel>(this, "App/", data: "Wellcome");
        }
       
    }

    private async void OpenAddTaskView()
    {
        await _navigator.NavigateViewModelAsync<AddTaskViewModel>(this, data: _store);
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
