using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App2.ViewModels;

public class TaskDetailViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public ICommand DeleteTaskCommand { get; }
    public Action<BaseTask>? OnTaskDetele { get; set; } // callback
    public Action? ShowDeleteDialog { get; set; }
    public BaseTask Task { get; }

    public TaskDetailViewModel(MainWindowViewModel main, BaseTask task)
    {
        _mainWindowViewModel = main;
        Task = task;
        DeleteTaskCommand = new RelayCommand(() => ShowDeleteDialog?.Invoke());
    }

    public void DeleteTask()
    {
        OnTaskDetele?.Invoke(Task);
        _mainWindowViewModel.MainView = new MainViewModel(_mainWindowViewModel);
    }
}
