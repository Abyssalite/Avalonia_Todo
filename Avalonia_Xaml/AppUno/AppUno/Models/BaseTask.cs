using System.ComponentModel;

public class BaseTask : INotifyPropertyChanged
{
    public required string Name { set; get; }
    public string? Description { set; get; }
    public required string Category { get; set; }
    public required string List { get; set; }

    private bool _isDone;
    public bool IsDone
    {
        get => _isDone;
        set
        {
            if (_isDone != value)
            {
                _isDone = value;
                OnPropertyChanged(nameof(IsDone));
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
