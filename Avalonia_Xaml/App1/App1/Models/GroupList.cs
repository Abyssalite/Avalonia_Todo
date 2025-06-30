using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

public class GroupList : INotifyPropertyChanged
{
    public required string ListName { get; set; }
    private bool _isArchived = false;
    public bool IsArchived
    {
        get => _isArchived;
        set
        {
            if (_isArchived != value)
            {
                _isArchived = value;
                OnPropertyChanged(nameof(IsArchived));
            }
        }
    }
    private ObservableCollection<TaskGroup> _groups = new();
    public required ObservableCollection<TaskGroup> Groups
    {
        get => _groups;
        set
        {
            _groups.CollectionChanged -= OnGroupsChanged;
            _groups = value;
            _groups.CollectionChanged += OnGroupsChanged;

            HookChanges();
            OnPropertyChanged(nameof(Groups));
        }
    }

    public GroupList()
    {
        if (Groups != null)
            Groups.CollectionChanged += OnGroupsChanged;
    }

    private void OnGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        HookChanges();

    private void HookChanges()
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

