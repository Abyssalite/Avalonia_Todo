using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class TaskGroupViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    public ObservableCollection<TaskGroup>? GroupedTasks { get; }
    public string? ListName { get; }
    public ICommand AddTaskViewCommand { get; }
    public ICommand DeleteListCommand { get; }
    private BaseTask? _selectedTask;
    public BaseTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (value != null && value != _selectedTask)
            {
                _selectedTask = value;
                OpenTask(value);
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
    }

    public TaskGroupViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
        if (store.SelectedList != null)
        {
            ListName = store.ListName;
            GroupedTasks = store.SelectedList.Groups;
        }

        AddTaskViewCommand = new RelayCommand(OpenAddTaskView);
        DeleteListCommand = new RelayCommand(DeleteList);
    }
    
        private void DeleteList()
    {
        if (ListName != null && ListName != "Quick")
        { 
            TaskHelpers.DeleteList(ListName, _store);
        }
    }

    private void OpenAddTaskView()
    {
        if (ListName != null)
        {
            _host.NavigateRight(new AddTaskViewModel(_host, _host.RightView, _store));
            _host.NavigateLeft(new NewTaskOptionViewModel(_host, _store));
        }
    }

    private void OpenTask(BaseTask task)
    {
        _selectedTask = null;
        OnPropertyChanged(nameof(SelectedTask));

        _store.SelectedTask = task;
        _host.NavigateRight(new TaskDetailViewModel(_host,_host.RightView, _store));
    } 
    
}
