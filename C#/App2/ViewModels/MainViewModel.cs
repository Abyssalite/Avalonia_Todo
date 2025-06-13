using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using App2.Views;
using System.Linq;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public TaskDetailViewModel? TaskDetailView { get; private set; }
    public ObservableCollection<TaskGroup> GroupedTasks { get; set; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
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

                SelectedTask = null; // reset to allow re-selection
            }
        }
    }

    public MainViewModel(MainWindowViewModel main)
    {
        _mainWindowViewModel = main;
        GroupedTasks = Load();
        HookSaveOnIsDoneChange();
        AddTaskViewCommand = new RelayCommand(OpenAddTaskView);
    }

    public void AddTaskToCategory(BaseTask task)
    {
        var group = GroupedTasks.FirstOrDefault(g => g.Category == task.Category);
        if (group != null)
        {
            group.Tasks.Add(task);
        }
        else
        {
            // If category doesn't exist, create a new one
            GroupedTasks.Add(new TaskGroup
            {
                Category = task.Category,
                Tasks = new ObservableCollection<BaseTask> { task }
            });
        }
    }

    public void DeleteTask(BaseTask task)
    {
        var group = GroupedTasks.FirstOrDefault(g => g.Category == task.Category);
        if (group != null)
        {
            group.Tasks.Remove(task);
            if (group.Tasks.Count == 0)
            {
                GroupedTasks.Remove(group);
            };
        }
    }

    private void HookSaveOnIsDoneChange()
    {
        foreach (var group in GroupedTasks)
        {
            foreach (var task in group.Tasks)
            {
                task.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(BaseTask.IsDone))
                    {
                        Save();
                    }
                };
            }
        }
    }

    private void OpenAddTaskView()
    {
        var addVM = new AddTaskViewModel(_mainWindowViewModel);
        addVM.OnTaskCreated = task =>
        {
            AddTaskToCategory(task);
            Save();
        };
        _mainWindowViewModel.MainView = addVM;
    }

    private void OpenTask(BaseTask task)
    {
        TaskDetailView = new TaskDetailViewModel(_mainWindowViewModel,task);
        TaskDetailView.OnTaskDetele = task =>
        {
            DeleteTask(task);
            Save();

        };
        OnPropertyChanged(nameof(TaskDetailView));
    }

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private void Save(string fileName = "tasks.json")
    {
        string json = JsonSerializer.Serialize(GroupedTasks, JsonOptions);
        File.WriteAllText(fileName, json);
        Console.WriteLine("Saving tasks.json to: " + Directory.GetCurrentDirectory());
    }
    
    private ObservableCollection<TaskGroup> Load(string filename = "tasks.json")
    {
        try
        {
            string json = File.ReadAllText(filename);
            Console.WriteLine($"Tasks loaded from {filename}.");
            return JsonSerializer.Deserialize<ObservableCollection<TaskGroup>>(json, JsonOptions) ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
            Console.WriteLine("Creating...");
            return [];
        }
    }
}
