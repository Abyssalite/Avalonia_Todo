using System.Collections.ObjectModel;
using System.Text.Json;
using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class GroupListViewModel : ViewModelBase
{
    private Store _store;
    private INavigator _navigator;
    public ObservableCollection<GroupList> FilteredGroupedList { get; } = new();
    public ICommand OpenQuickTaskCommand { get; }
    public ICommand? GoToSecond { get; }
    private GroupList? _selectedGroup;
    public GroupList? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (value != null && value != _selectedGroup)
            {
                _selectedGroup = value;
                OpenGroup(_selectedGroup);
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }
    }
    public GroupListViewModel(Store store, INavigator nav)
    {
        _navigator = nav;
        _store = store;
        FilteredGroupedList = _store.FilteredGroupedList;

        OpenQuickTaskCommand = new RelayCommand(() =>
        {
            var quick = _store.GroupedList.FirstOrDefault(l => l.List == "Quick");
            if (quick != null)
                OpenGroup(quick);
            _selectedGroup = null;
            OnPropertyChanged(nameof(SelectedGroup));
        });
    }
    private async void OpenGroup(GroupList groupedList)
    {
        await _navigator.NavigateRouteAsync(this, "/Right/TaskGroupView", data:new Entity.TaskGroup(_store, groupedList));
    }
    
}
