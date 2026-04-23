using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class GroupList : INotifyPropertyChanged
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
            OnPropertyChanged(nameof(IsArchived));
        }
    }

    private ObservableCollection<TaskGroup> _groups = new();
    public required ObservableCollection<TaskGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value ?? new();
            OnPropertyChanged(nameof(Groups));
        }
    }

    public GroupList() {}

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}