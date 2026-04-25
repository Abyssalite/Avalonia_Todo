using System;
using System.Collections.ObjectModel;
using System.Linq;
using App1.Events;
using App1.ViewModels;
using Avalonia_EventHub;

public class TaskGroup : ModelBase
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
            _events.Publish(new TaskGroupChangedEvent(value));
        }
    }

    public TaskGroup(IEventHub events) : base (events)
    {}

    public TaskGroup(TaskGroup other, IEventHub events) : base (events)
    {
        Category = other.Category;
        Tasks = new ObservableCollection<BaseTask>(other.Tasks.Select(task => new BaseTask(task, _events)
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