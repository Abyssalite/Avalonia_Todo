using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class TaskDetailViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly TaskGroupViewModel _taskGroupViewModel;
    public BaseTask Task { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand BackCommand { get; }
    public Action? ShowDeleteDialog { get; set; }
    public Action<BaseTask>? OnTaskDetele { get; set; }

    public TaskDetailViewModel(MainViewModel main, TaskGroupViewModel taskGroupViewModel, BaseTask task)
    {
        _mainViewModel = main;
        _taskGroupViewModel = taskGroupViewModel;
        Task = task;
        DeleteTaskCommand = new RelayCommand(() => ShowDeleteDialog?.Invoke());
        BackCommand = new RelayCommand(() =>  _mainViewModel.SideView = _taskGroupViewModel);
    }

    public void DeleteTask()
    {
        OnTaskDetele?.Invoke(Task);
         _mainViewModel.SideView = _taskGroupViewModel;
    }
}
