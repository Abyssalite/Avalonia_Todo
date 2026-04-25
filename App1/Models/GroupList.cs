using System;
using System.Collections.ObjectModel;
using App1.Events;
using App1.ViewModels;
using Avalonia_EventHub;

public class GroupList : ModelBase
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
            _events.Publish(new GroupListIsArchiveStateChangedEvent(this, value));
        }
    }

    private ObservableCollection<TaskGroup> _groups = new();
    public required ObservableCollection<TaskGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            _events.Publish(new GroupListChangedEvent(value));
        }
    }

    public GroupList(IEventHub events) : base (events)
    {}

}