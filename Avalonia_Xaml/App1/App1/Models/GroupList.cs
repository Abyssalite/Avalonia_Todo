using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

public class GroupList : INotifyPropertyChanged
{
    public required string List { get; set; }
    private ObservableCollection<TaskGroup> _groups = new();
    public required ObservableCollection<TaskGroup> Groups
    {
        get => _groups;
        set
        {
            _groups.CollectionChanged -= OnGroupsChanged;
            _groups = value;
            _groups.CollectionChanged += OnGroupsChanged;

            UpdateGroups();
            OnPropertyChanged(nameof(Groups));
        }
    }

    public GroupList()
    {
        if (Groups != null)
            Groups.CollectionChanged += OnGroupsChanged;
    }

    private void OnGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        UpdateGroups();

    private void UpdateGroups()
    {
        foreach (var group in Groups)
            group.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(BaseTask.IsDone))
                    OnPropertyChanged(nameof(BaseTask.IsDone));
            };
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

