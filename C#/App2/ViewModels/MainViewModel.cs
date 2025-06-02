using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public ObservableCollection<BaseTask> Tasks { get; } = new();

    public ICommand OpenTaskCommand { get; }

    public MainViewModel(MainWindowViewModel main)
    {
        Console.WriteLine("MainViewModel");
        _mainWindowViewModel = main;
        Tasks.Add(new BaseTask { Name = "Test Task", Description = "Details here", Category = "Work", IsDone = false });

        OpenTaskCommand = new RelayCommand<BaseTask>(OpenTask);

    }

    private void OpenTask(BaseTask? task)
    {
        if (task is not null)
            _mainWindowViewModel.MainView = new TaskDetailViewModel(_mainWindowViewModel, task);
    }
}
