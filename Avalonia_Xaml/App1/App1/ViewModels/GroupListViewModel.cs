using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class GroupListViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    public ObservableCollection<GroupList>? FilteredGroupedList { get; set; } = new();
    public ICommand OpenQuickTaskCommand { get; }
    public ICommand AddListCommand { get; }
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
                _ = OpenGroupAsync(_selectedGroup);
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

        OpenQuickTaskCommand = new RelayCommand(async () =>
        {
            var quick = _store.GroupedList.FirstOrDefault(l => l.List == "Quick");
            if (quick != null)
                await OpenGroupAsync(quick);
            _selectedGroup = null;
            OnPropertyChanged(nameof(SelectedGroup));
        });
        AddListCommand = new AsyncRelayCommand<string>(AddList);
    }

    private async Task AddList(string? newListName)
    {
        OnSaveAddList?.Invoke();
        if (newListName != null)
        {
            await TaskHelpers.AddList(newListName, _store);
        }
    }

    private async Task OpenGroupAsync(GroupList groupedList)
    {
        if (_store.ListName != groupedList.List)
        {
            _store.SelectedList = groupedList;
            _store.ListName = groupedList.List;
            await _host.NavigateRight(new TaskGroupViewModel(_host, _store));
        }
    }
}