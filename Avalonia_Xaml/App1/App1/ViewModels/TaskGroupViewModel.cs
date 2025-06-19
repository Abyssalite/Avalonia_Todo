using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class TaskGroupViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly GroupListViewModel _groupListViewModel;
    private Store _store { get; }
    public ObservableCollection<TaskGroup> GroupedTasks { get; }
    public string ListName { get; }
    public ICommand AddTaskViewCommand { get; } //Button only
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

    public TaskGroupViewModel(MainViewModel main, GroupListViewModel groupList, Store store, GroupList list)
    {
        _mainViewModel = main;
        _groupListViewModel = groupList;
        _store = store;
        ListName = list.List;
        GroupedTasks = list.Groups;
        AddTaskViewCommand = new RelayCommand(OpenAddTaskView);
    }

    private void OpenAddTaskView()
    {
        if (ListName != null)
        {
            var addVM = new AddTaskViewModel(_mainViewModel, _groupListViewModel ,this, ListName);
            addVM.OnTaskCreated = task =>
            {
                TaskHelpers.AddTaskToCategory(task, _store.GroupedList);
            };
            _mainViewModel.RightView = addVM;
            OnPropertyChanged(nameof(_mainViewModel.RightView));
            _mainViewModel.LeftView = new NewTaskOptionViewModel(_mainViewModel, _groupListViewModel);
            OnPropertyChanged(nameof(_mainViewModel.LeftView));
        }
    }

    private void OpenTask(BaseTask task)
    {
        _selectedTask = null;
        OnPropertyChanged(nameof(SelectedTask));
        var TaskDetailView = new TaskDetailViewModel(_mainViewModel, this, task);
        TaskDetailView.OnTaskDetele = task =>
        {
            _store.GroupedList = TaskHelpers.DeleteTask(task, _store.GroupedList);
        };
        _mainViewModel.RightView = TaskDetailView;
        OnPropertyChanged(nameof(_mainViewModel.RightView));

    }
    
}
