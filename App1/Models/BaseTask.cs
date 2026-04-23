using System;
using System.ComponentModel;

public class BaseTask : INotifyPropertyChanged
{
    public Guid ID = Guid.NewGuid();
    public required string Name { set; get; }
    public string? Description { set; get; }
    public required string Category { get; set; }
    public required string ListName { get; set; }
    private bool _isDone;
    public bool IsDone
    {
        get => _isDone;
        set
        {
            if (_isDone == value) return;
            _isDone = value;
            OnPropertyChanged(nameof(IsDone));
        }
    }
    private bool _isImportant;
    public bool IsImportant
    {
        get => _isImportant;
        set
        {
            if (_isImportant == value) return;
            _isImportant = value;
            OnPropertyChanged(nameof(IsImportant));
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
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
