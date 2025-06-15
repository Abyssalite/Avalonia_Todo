using System.Collections.ObjectModel;

public class GroupList
{
    public required string List { get; set; }
    public required ObservableCollection<TaskGroup> Group { get; set; }
}
