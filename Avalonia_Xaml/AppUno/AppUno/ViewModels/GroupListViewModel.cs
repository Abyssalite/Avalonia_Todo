using System.Collections.ObjectModel;
using AppUno.ViewModels;

namespace AppUno.ViewsModels;

public class GroupListViewModel : ViewModelBase
{
    private Store _store;
    private INavigator _navigator;
    public ObservableCollection<GroupList>? FilteredGroupedList { get; set; } = new();
    public ICommand OpenQuickTaskCommand { get; }
    public ICommand? AddListCommand { get; }
    public Action? OnSaveAddList { get; set; }
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
        _store.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Store.FilteredGroupedList))
            {
                FilteredGroupedList.Clear();
                FilteredGroupedList = _store.FilteredGroupedList;
                OnPropertyChanged(nameof(FilteredGroupedList));
            }
        };

        OpenQuickTaskCommand = new RelayCommand(() =>
        {
            var quick = _store.GroupedList.FirstOrDefault(l => l.List == "Quick");
            if (quick != null)
                OpenGroup(quick);
            _selectedGroup = null;
            OnPropertyChanged(nameof(SelectedGroup));
        });
        AddListCommand = new RelayCommand<string>(AddList);
    }
    
    private void AddList(string? newListName)
    {
        OnSaveAddList?.Invoke();
        if (newListName != null)
        {
            TaskHelpers.AddList(newListName, _store);

        }
    }
    
    private async void OpenGroup(GroupList groupedList)
    {
        _store.SelectedList = groupedList;
        _store.ListName = groupedList.List;
        await _navigator.NavigateViewModelAsync<TaskGroupViewModel>(this, "App/", data: _store);
    }
}