using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

public class ArchivedList : INotifyPropertyChanged
{
    private ObservableCollection<GroupList> _archivedLists = new();
    public ObservableCollection<GroupList> ArchivedLists
    {
        get => _archivedLists;
        set
        {
            if (value != null)
            {
                _archivedLists.CollectionChanged -= OnArchivedListChanged;
                _archivedLists = value;
                _archivedLists.CollectionChanged += OnArchivedListChanged;

                OnPropertyChanged(nameof(ArchivedLists));
            }
        }
    }

    public ArchivedList()
    {
        ArchivedLists.CollectionChanged += OnArchivedListChanged;
    }

    private void OnArchivedListChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        OnPropertyChanged(nameof(ArchivedLists));

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}