using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;

namespace App1.ViewModels;

public class TaskGroupViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    public ObservableCollection<TaskGroup> GroupedTasks { get; set; }
    public ICommand AddTaskViewCommand { get; } //Button only
    public Action<BaseTask>? OnTaskDetele { get; set; }
    public Action<TaskGroupViewModel, string>? OnTaskCreate { get; set; } // callback
    public string ListName { get; set; }
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
   
    public TaskGroupViewModel(MainViewModel main, ObservableCollection<TaskGroup> groups, string listName)
    {
        _mainViewModel = main;
        ListName = listName;
        GroupedTasks = groups;
        AddTaskViewCommand = new RelayCommand(OpenAddTaskView);
    }

    private void OpenAddTaskView()
    {
        if (ListName != null)
        {
            OnTaskCreate?.Invoke(this, ListName);
        }
    }

    private void OpenTask(BaseTask task)
    {
        _selectedTask = null;
        OnPropertyChanged(nameof(SelectedTask));
        var TaskDetailView = new TaskDetailViewModel(_mainViewModel, this, task);
        TaskDetailView.OnTaskDetele = task =>
        {
            OnTaskDetele?.Invoke(task);
        };
        _mainViewModel.SideView = TaskDetailView;
        OnPropertyChanged(nameof(_mainViewModel.SideView));

    }
}
