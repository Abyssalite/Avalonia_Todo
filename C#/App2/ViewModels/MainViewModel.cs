using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using App2.Views;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public ViewModelBase? TaskDetailView { get; private set; }

    public ObservableCollection<BaseTask> Tasks { get; set; } = new();
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
            if (value is not null &&_selectedTask != value)
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                OpenTask(value);
            }
        }
    }

    public MainViewModel(MainWindowViewModel main)
    {
        _mainWindowViewModel = main;
        Tasks = Load();
        AddTaskViewCommand = new RelayCommand(OpenAddTaskView);
    }

    private void OpenAddTaskView()
    {
        var addVM = new AddTaskViewModel(_mainWindowViewModel);
        addVM.OnTaskCreated = task =>
        {
            Tasks.Add(task);
            Save();
        };
        _mainWindowViewModel.MainView = addVM;
    }

    private void OpenTask(BaseTask task)
    {
        TaskDetailView = new TaskDetailViewModel(task);
        OnPropertyChanged(nameof(TaskDetailView));
    }

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private void Save(string fileName = "tasks.json")
    {
        string json = JsonSerializer.Serialize(Tasks, JsonOptions);
        File.WriteAllText(fileName, json);
        Console.WriteLine("Saving tasks.json to: " + Directory.GetCurrentDirectory());
    }

    private ObservableCollection<BaseTask> Load(string filename = "tasks.json")
    {
        try
        {
            string json = File.ReadAllText(filename);
            Console.WriteLine($"Tasks loaded from {filename}.");
            return JsonSerializer.Deserialize<ObservableCollection<BaseTask>>(json, JsonOptions) ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
            Console.WriteLine("Creating...");
            return [];
        }
    }
}
