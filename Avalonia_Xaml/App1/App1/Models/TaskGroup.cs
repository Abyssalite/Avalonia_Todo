using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

public class TaskGroup : INotifyPropertyChanged
{
    public required string Category { get; set; }
    private ObservableCollection<BaseTask> _tasks = new();
    public required ObservableCollection<BaseTask> Tasks
    {
        get => _tasks;
        set
        {
            _tasks.CollectionChanged -= OnTasksChanged;
            _tasks = value;
            _tasks.CollectionChanged += OnTasksChanged;

            HookChanges();
            OnPropertyChanged(nameof(Tasks));
        }
    }

    public TaskGroup()
    {
        if (Tasks != null)
            Tasks.CollectionChanged += OnTasksChanged;
    }

    private void OnTasksChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        HookChanges();

    private void HookChanges()
    {
        foreach (var task in Tasks)
            task.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(BaseTask.IsDone))
                    OnPropertyChanged(nameof(BaseTask.IsDone));
            };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
