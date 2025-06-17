using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class AddTaskViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly TaskGroupViewModel _taskGroupViewModel;
    private readonly string _listName;
    public Action<BaseTask>? OnTaskCreated { get; set; } // callback
    public Action? ShowEmptyNameDialog { get; set; }
    public ICommand SaveTaskCommand { get; } //Button only
    public ICommand CancelCommand { get; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
    public string? GroupList { get; set; }

    private string InputOrDefault(string? input, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    public AddTaskViewModel(MainViewModel main, TaskGroupViewModel taskGroupViewModel, string listName)
    {
        _mainViewModel = main;
        _taskGroupViewModel = taskGroupViewModel;
        _listName = listName;
        SaveTaskCommand = new RelayCommand(AddTask);
        CancelCommand = new RelayCommand(Clear);
    }

    private void Clear()
    {
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
        _mainViewModel.SideView = _taskGroupViewModel;
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
            List = _listName,
            Category = InputOrDefault(TaskCatalog, "Miscelanious"),
            Description = InputOrDefault(TaskDesc, "")
        };
        OnTaskCreated?.Invoke(task); // Call back to MainViewModel
        Clear();
    }
}
