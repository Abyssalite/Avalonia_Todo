using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

public class Store : INotifyPropertyChanged
{
    private ObservableCollection<GroupList> _groupedList = new();
    public string? ListName { set; get; }
    public string WellcomeText { set; get; } = "Wellcome";
    public bool Initialized { set; get; } = false;
    public GroupList? SelectedList { get; set; }
    public BaseTask? SelectedTask { get; set; }
    public ObservableCollection<GroupList> FilteredGroupedList { get; set; } = new();
    public ObservableCollection<GroupList> GroupedList
    {
        get => _groupedList;
        set
        {
            if (value != null)
            {
                _groupedList.CollectionChanged -= OnGroupedListChanged;
                _groupedList = value;
                _groupedList.CollectionChanged += OnGroupedListChanged;

                UpdateFilteredList();
                OnPropertyChanged(nameof(GroupedList));
            }
        }
    }
    
    public Store()
    {
        GroupedList.CollectionChanged += OnGroupedListChanged;
    }

    private void OnGroupedListChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        UpdateFilteredList();
    
    private void UpdateFilteredList()
    {
        foreach (var list in GroupedList)
            list.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(BaseTask.IsDone))
                    OnPropertyChanged(nameof(BaseTask.IsDone));
            };
            
        FilteredGroupedList = new ObservableCollection<GroupList>(
            GroupedList.Where(g => g.List != "Quick")
        );
        OnPropertyChanged(nameof(FilteredGroupedList));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
