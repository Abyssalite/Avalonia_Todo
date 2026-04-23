using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

public class TaskGroup : INotifyPropertyChanged
{
    public Guid ID = Guid.NewGuid();
    public required string Category { get; set; }

    private ObservableCollection<BaseTask> _tasks = new();
    public required ObservableCollection<BaseTask> Tasks
    {
        get => _tasks;
        set
        {
            _tasks = value ?? new();
            OnPropertyChanged(nameof(Tasks));
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}