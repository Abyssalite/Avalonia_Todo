using System;
using System.Collections.ObjectModel;

public class ArchivedList
{
    public Guid ID = Guid.NewGuid();
    public ObservableCollection<GroupList> ArchivedLists { get; set; } = new();

    public ArchivedList() {}
}