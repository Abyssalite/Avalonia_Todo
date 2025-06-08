using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App2.ViewModels;

public class AddTaskViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public Action<BaseTask>? OnTaskCreated { get; set; } // callback
    public Action? ShowEmptyNameDialog { get; set; }

    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
    public ICommand SaveTaskCommand { get; } //Button only
    public ICommand CancelCommand { get; }


    private string InputOrDefault(string? input, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    public AddTaskViewModel(MainWindowViewModel main)
    {
        _mainWindowViewModel = main;
        SaveTaskCommand = new RelayCommand(AddTask);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Cancel()
    {
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
        _mainWindowViewModel.MainView = new MainViewModel(_mainWindowViewModel);

    }
    private void AddTask()
    {

        string name = InputOrDefault(NewTaskName, "");
        if (name == "")
        {
            ShowEmptyNameDialog?.Invoke(); 
            return;
        }
        var task = new BaseTask

        {
            Name = name,
            IsDone = false,
            Category = InputOrDefault(TaskCatalog, "Miscelanious"),
            Description = InputOrDefault(TaskDesc, "")
        };
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));

        OnTaskCreated?.Invoke(task); // Call back to MainViewModel
        _mainWindowViewModel.MainView = new MainViewModel(_mainWindowViewModel);
    }
}
