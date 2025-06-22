using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

public class Store : INotifyPropertyChanged
{
    private ObservableCollection<GroupList> _groupedList = new();
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
                _groupedList = value;
                FilteredGroupedList = new ObservableCollection<GroupList>(value.Where(g => g.List != "Quick"));
                OnPropertyChanged(nameof(GroupedList));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
