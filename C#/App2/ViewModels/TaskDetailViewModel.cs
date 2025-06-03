using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App2.ViewModels;

public class TaskDetailViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public BaseTask Task { get; }

    public ICommand GoBackCommand { get; }

    public TaskDetailViewModel(MainWindowViewModel main, BaseTask task)
    {
        _mainWindowViewModel = main;
        Task = task;
        GoBackCommand = new RelayCommand(() => _mainWindowViewModel.MainView = new MainViewModel(_mainWindowViewModel));
    }
}
