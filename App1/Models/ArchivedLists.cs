using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class ArchivedList : INotifyPropertyChanged
{
    public Guid ID = Guid.NewGuid();
    private ObservableCollection<GroupList> _archivedLists = new();
    public ObservableCollection<GroupList> ArchivedLists
    {
        get => _archivedLists;
        set
        {
            if (value == null) return;
            _archivedLists = value;
            OnPropertyChanged(nameof(ArchivedLists));
        }
    }

    public ArchivedList() {}

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}