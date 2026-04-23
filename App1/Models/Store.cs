using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia_EventHub;
using App1.Events;

public class Store : INotifyPropertyChanged
{
    private readonly IEventHub _events;

    private ObservableCollection<GroupList> _lists = new();
    private string _topBarText = "";

    public Store(IEventHub events)
    {
        _events = events;
        Archive = new();
    }

    public string SelectedListName { get; private set; } = "";
    public GroupList? SelectedList { get; private set; }
    public BaseTask? SelectedTask { get; set; }
    public ArchivedList Archive { get; }
    public string WelcomeText { get; set; } = "Welcome";
    public bool Initialized { get; set; }

    public ObservableCollection<GroupList> FilteredLists =>
        new(_lists.Where(g => g.ListName != GlobalVariables.Quick));

    public ObservableCollection<GroupList> Lists
    {
        get => _lists;
        set
        {
            _lists = value ?? new();
            OnPropertyChanged(nameof(Lists));
            OnPropertyChanged(nameof(FilteredLists));
            _events.Publish(new ListsChangedEvent());
        }
    }

    public string TopbarText
    {
        get => _topBarText;
        set
        {
            if (_topBarText == value) return;
            _topBarText = value;
            OnPropertyChanged(nameof(TopbarText));
            _events.Publish(new TopbarTextChangedEvent(_topBarText));
        }
    }

    public void SelectList(GroupList? list)
    {
        SelectedList = list;
        SelectedListName = list?.ListName ?? "";
        OnPropertyChanged(nameof(SelectedList));
        OnPropertyChanged(nameof(SelectedListName));
        _events.Publish(new SelectedListChangedEvent(SelectedList, SelectedListName));
    }

    public void NotifyListsChanged()
    {
        OnPropertyChanged(nameof(Lists));
        OnPropertyChanged(nameof(FilteredLists));
        _events.Publish(new ListsChangedEvent());
    }

    public void NotifyArchiveChanged()
    {
        OnPropertyChanged(nameof(Archive));
        _events.Publish(new ArchiveListsChangedEvent());
    }

    public void NotifyArchiveToggled()
    {
        OnPropertyChanged(nameof(GroupList.IsArchived));
        _events.Publish(new ArchiveListsChangedEvent());
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}