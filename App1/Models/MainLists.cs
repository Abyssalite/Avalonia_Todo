using System;
using System.Collections.ObjectModel;

public class MainList
{
    public Guid ID = Guid.NewGuid();
    public ObservableCollection<GroupList> MainLists { get; set; } = new();

    public MainList() {}
}