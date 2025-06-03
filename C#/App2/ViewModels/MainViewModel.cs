using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public ObservableCollection<BaseTask> Tasks { get; set; } = new();
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
    public ICommand OpenTaskCommand { get; }
    public ICommand AddTaskCommand { get; }

    private string InputOrDefault(string? input, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    public MainViewModel(MainWindowViewModel main)
    {
        _mainWindowViewModel = main;
        Tasks = Load();
        OpenTaskCommand = new RelayCommand<BaseTask>(OpenTask);
        AddTaskCommand = new RelayCommand(AddTask);
    }

    private void OpenTask(BaseTask? task)
    {
        if (task is not null)
            _mainWindowViewModel.MainView = new TaskDetailViewModel(_mainWindowViewModel, task);
    }

    private void AddTask()
    {
        string name = InputOrDefault(NewTaskName, "");
        if (name != "")
            Tasks.Add(new BaseTask
            {
                Name = name,
                IsDone = false,
                Category = InputOrDefault(TaskCatalog, ""),
                Description = InputOrDefault(TaskDesc, "")
            });
        Save();
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
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
