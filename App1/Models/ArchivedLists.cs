using System;
using System.Collections.ObjectModel;
using App1.ViewModels;
using Avalonia_EventHub;

public class ArchivedList : ModelBase
{
    public Guid ID = Guid.NewGuid();
    public ObservableCollection<GroupList> ArchivedLists { get; set; } = new();

    public ArchivedList(IEventHub events) : base (events)
    {}
}