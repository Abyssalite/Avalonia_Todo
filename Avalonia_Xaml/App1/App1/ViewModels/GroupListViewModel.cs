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
                _ = OpenListAsync(_selectedGroup);
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }
    }

    public GroupListViewModel(IViewHost host, Store store, INotificationService notificate, IPaneService paneService)
    {
        _store = store;
        _host = host;
        Notificate = notificate;
        _paneService = paneService;
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
                await OpenListAsync(quick);
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
            bool isExisted = await TaskHelpers.AddList(newListName, _store);
            if (isExisted) Notificate.ShowNotification("The list is Existed");
        }
    }

    private async Task OpenListAsync(GroupList groupedList)
    {
        if (_store.ListName != groupedList.List)
        {
            _store.SelectedList = groupedList;
            _store.ListName = groupedList.List;
            await _host.NavigateRight(App.Services?.GetRequiredService<TaskGroupViewModel>());
            _paneService.OpenPane(false);
        }
    }
}