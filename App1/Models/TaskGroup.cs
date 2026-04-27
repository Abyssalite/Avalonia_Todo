using System;
using System.Collections.ObjectModel;
using System.Linq;
using App1.Events;
using App1.ViewModels;
using Avalonia_EventHub;

public class TaskGroup
{
    public Guid ID = Guid.NewGuid();
    public required string Category { get; set; }

    private ObservableCollection<BaseTask> _tasks = new();
    public required ObservableCollection<BaseTask> Tasks
    {
        get => _tasks;
        set
        {
            _tasks = value;
        }
    }

    public TaskGroup() {}

    public TaskGroup(TaskGroup other)
    {
        Category = other.Category;
        Tasks = new ObservableCollection<BaseTask>(other.Tasks.Select(task => new BaseTask(task)
        {
            ID = task.ID,
            Name = task.Name,
            Category = task.Category,
            ListName = task.ListName,
            Description = task.Description,
            IsDone = task.IsDone,
            IsImportant = task.IsImportant
        }));
    }
}