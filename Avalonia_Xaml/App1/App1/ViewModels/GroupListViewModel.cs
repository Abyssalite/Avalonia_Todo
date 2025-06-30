using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace App1.ViewModels;

public partial class GroupListViewModel : ViewModelBase
{
    private readonly Store _store;
    private readonly IViewHost _host;
    public INotificationService Notificate  { get; set; }
    private readonly IPaneService _paneService;
    public ObservableCollection<GroupList>? FilteredLists { get; set; } = new();
    public ICommand OpenTaskCommand { get; }
    public ICommand AddListCommand { get; }
    public Action? OnSaveAddList { get; set; }
    private bool _toggleArchive = false;
    public bool ToggleArchive
    {
        get => _toggleArchive;
        set
        {
            _toggleArchive = value;
            if(_toggleArchive) FilteredLists = _store.Archive.ArchivedLists;
            else FilteredLists = _store.FilteredLists;

            OnPropertyChanged(nameof(FilteredLists));
            OnPropertyChanged(nameof(ToggleArchive));
        }
    } 
    private GroupList? _selectedList;
    public GroupList? SelectedList
    {
        get => _selectedList;
        set
        {
            if (value != null && value != _selectedList)
            {
                _selectedList = value;
                _ = OpenListAsync(_selectedList);
                OnPropertyChanged(nameof(SelectedList));
            }
        }
    }

    public GroupListViewModel(IViewHost host, Store store, INotificationService notificate, IPaneService paneService)
    {
        _store = store;
        _host = host;
        Notificate = notificate;
        _paneService = paneService;
        FilteredLists = _store.FilteredLists;
        _store.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Store.FilteredLists) && !ToggleArchive)
            {
                FilteredLists.Clear();
                FilteredLists = _store.FilteredLists;
                OnPropertyChanged(nameof(FilteredLists));
            }
        };

        OpenTaskCommand = new RelayCommand<string>(async (listName) =>
        {
            var list = _store.Lists.FirstOrDefault(l => l.ListName == listName);
            if (list != null)
                await OpenListAsync(list);
            _selectedList = null;
            OnPropertyChanged(nameof(SelectedList));
        });
        AddListCommand = new AsyncRelayCommand<string>(AddList);
    }

    private async Task AddList(string? newListName)
    {
        OnSaveAddList?.Invoke();
        if (newListName != null)
        {
            bool isExisted = await TaskHelpers.AddList(newListName, _store);
            if (isExisted) Notificate.ShowNotification("The list is Existed");
        }
    }

    private async Task OpenListAsync(GroupList groupedList)
    {
        if (_store.SelectedListName != groupedList.ListName)
        {
            _store.SelectedList = groupedList;
            _store.SelectedListName = groupedList.ListName;
            await _host.NavigateRight(App.Services?.GetRequiredService<TaskGroupViewModel>());
            _paneService.OpenPane(false);
        }
    }
}