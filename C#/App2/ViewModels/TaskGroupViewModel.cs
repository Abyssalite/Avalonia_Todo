using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;
using App2.Views;

namespace App2.ViewModels;

public class TaskGroupViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public TaskDetailViewModel? TaskDetailView { get; private set; }
    public ObservableCollection<TaskGroup> GroupedTasks { get; set; }
    public ICommand AddTaskViewCommand { get; } //Button only
    public Action<BaseTask>? OnTaskDetele { get; set; }
    public Action<string>? OnTaskCreate { get; set; } // callback
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

                SelectedTask = null; // reset to allow re-selection
            }
        }
    }

    public TaskGroupViewModel(MainWindowViewModel main, ObservableCollection<TaskGroup> groups)
    {
        _mainWindowViewModel = main;
        GroupedTasks = groups;
        AddTaskViewCommand = new RelayCommand<string>(OpenAddTaskView);
    }

    private void OpenAddTaskView(string? list)
    {
        if (list != null)
        {
            OnTaskCreate?.Invoke(list);
        }
    }

    private void OpenTask(BaseTask task)
    {
        TaskDetailView = new TaskDetailViewModel(_mainWindowViewModel,task);
        TaskDetailView.OnTaskDetele = task =>
        {
            OnTaskDetele?.Invoke(task);
        };
        OnPropertyChanged(nameof(TaskDetailView));
    }
}
