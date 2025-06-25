using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class GroupListViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    public ObservableCollection<GroupList>? FilteredGroupedList { get; set; } = new();
    public ICommand OpenQuickTaskCommand { get; } //Button only
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

    public GroupListViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
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

    private void OpenGroup(GroupList groupedList)
    {
        if (_store.ListName != groupedList.List)
        {
            _store.SelectedList = groupedList;
            _store.ListName = groupedList.List;
            _host.NavigateRight(new TaskGroupViewModel(_host, _store));
        }
    }
}