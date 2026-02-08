using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

public class Store : INotifyPropertyChanged
{
    private ObservableCollection<GroupList> _lists = new();
    public string SelectedListName { set; get; } = "";
    private string _topBarText = "";
    public string TopbarText
    {
        get => _topBarText;
        set
        {
            if (value != null)
            {
                _topBarText = value;
                OnPropertyChanged(nameof(TopbarText));
            }
        }
    }
    public string WelcomeText { set; get; } = "Welcome";
    public bool Initialized { set; get; } = false;
    public GroupList? SelectedList { get; set; }
    public BaseTask? SelectedTask { get; set; }
    public ArchivedList Archive { set; get; }
    public ObservableCollection<GroupList> FilteredLists { get; set; } = new();
    public ObservableCollection<GroupList> Lists
    {
        get => _lists;
        set
        {
            if (value != null)
            {
                _lists.CollectionChanged -= OnListsChanged;
                _lists = value;
                _lists.CollectionChanged += OnListsChanged;

                HookChanges();
                OnPropertyChanged(nameof(Lists));
            }
        }
    }

    public Store()
    {
        Lists.CollectionChanged += OnListsChanged;
        Archive = new();
    }

    private void OnListsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        HookChanges();
    
    private void HookChanges()
    {
        foreach (var list in Lists)
            list.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(BaseTask.IsDone))
                    OnPropertyChanged(nameof(BaseTask.IsDone));
                if (e.PropertyName == nameof(BaseTask.IsImportant))
                    OnPropertyChanged(nameof(BaseTask.IsImportant));
                if (e.PropertyName == nameof(GroupList.IsArchived))
                    OnPropertyChanged(nameof(GroupList.IsArchived));
            };
            Archive.PropertyChanged +=  (_, e) =>
            {
                if (e.PropertyName == nameof(ArchivedList.ArchivedLists))
                    OnPropertyChanged(nameof(ArchivedList.ArchivedLists));
            };
        UpdateFilteredList();
    }

    private void UpdateFilteredList()
    {
        FilteredLists = new ObservableCollection<GroupList>(
            Lists.Where(g => g.ListName != GlobalVariables.Quick)
        );
        OnPropertyChanged(nameof(FilteredLists));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
