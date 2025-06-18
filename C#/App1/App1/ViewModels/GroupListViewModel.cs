using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class GroupListViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private Store _store { get; }
    public ObservableCollection<GroupList> FilteredGroupedList { get; } = new();
    public ICommand OpenQuickTaskCommand { get; } //Button only
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

    public GroupListViewModel(MainViewModel main, Store store)
    {
        _mainViewModel = main;
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

    private void OpenGroup(GroupList groupedList)
    {
        var TaskGroupView = new TaskGroupViewModel(_mainViewModel, this, _store, groupedList);
        _mainViewModel.RightView = TaskGroupView;
        OnPropertyChanged(nameof(_mainViewModel.RightView));
    }
}