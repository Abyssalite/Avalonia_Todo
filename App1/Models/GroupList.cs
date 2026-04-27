using System;
using System.Collections.ObjectModel;

public class GroupList
{
    public Guid ID = Guid.NewGuid();
    public required string ListName { get; set; }

    private bool _isArchived;
    public bool IsArchived
    {
        get => _isArchived;
        set
        {
            if (_isArchived == value) return;
            _isArchived = value;
            //_events.Publish(new GroupListIsArchiveStateChangedEvent(this, value));
        }
    }

    private ObservableCollection<TaskGroup> _groups = new();
    public required ObservableCollection<TaskGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
        }
    }

    public GroupList() {}

}