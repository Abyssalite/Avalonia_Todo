using System.Collections.ObjectModel;

public class TaskGroup
{
    public required string Category { get; set; }
    public required ObservableCollection<BaseTask> Tasks { get; set; }
}
