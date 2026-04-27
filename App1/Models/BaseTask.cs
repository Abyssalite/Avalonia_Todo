using System;

public class BaseTask
{
    public Guid ID = Guid.NewGuid();
    public required string Name { set; get; }
    public string? Description { set; get; }
    public required string Category { get; set; }
    public required string ListName { get; set; }
    private bool? _isDone;
    public bool? IsDone
    {
        get => _isDone;
        set
        {
            if (_isDone == value) return;
            _isDone = value;
            //_events.Publish(new TaskIsDoneChangedEvent(this, value));
        }
    }
    private bool? _isImportant;
    public bool? IsImportant
    {
        get => _isImportant;
        set
        {
            if (_isImportant == value) return;
            _isImportant = value;
            //_events.Publish(new TaskIsImportantChangedEvent(this, value));
        }
    }
    
    public BaseTask() {}
    public BaseTask(BaseTask other)
    {
        ID = other.ID;
        Name = other.Name;
        Category = other.Category;
        ListName = other.ListName;
        Description = other.Description;
        IsDone = other.IsDone;
        IsImportant = other.IsImportant;
    }

}
