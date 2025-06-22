using System.Collections.ObjectModel;
using System.Text.Json;
using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class TaskGroupViewModel : ViewModelBase
{
    public INavigator _navigator;
    private Store _store { get; }
    public ObservableCollection<TaskGroup> GroupedTasks { get; }
    public string ListName { get; }
    public ICommand? AddTaskViewCommand { get; } //Button only
    public ICommand OpenTaskCommand { get; }
    public TaskGroupViewModel(Entity.TaskGroup entity, INavigator nav)
    {
        _navigator = nav;
        _store = entity.store;
        ListName = entity.groupedList.List;
        GroupedTasks = entity.groupedList.Groups;

        OpenTaskCommand = new RelayCommand<BaseTask>(OpenTask);
    }

    private void OpenTask(BaseTask? task)
    {
                    Console.WriteLine(JsonSerializer.Serialize(task, TaskHelpers.JsonOptions));


    }
}
