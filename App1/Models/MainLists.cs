using System;
using System.Collections.ObjectModel;
using App1.Events;
using App1.ViewModels;
using Avalonia_EventHub;

public class MainList : ModelBase
{
    public Guid ID = Guid.NewGuid();
    public ObservableCollection<GroupList> MainLists { get; set; } = new();

    public MainList(IEventHub events) : base (events)
    {}
}