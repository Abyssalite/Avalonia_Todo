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
    public ObservableCollection<GroupList>? FilteredLists { get; set; } = new();
    public ICommand OpenListCommand { get; }
    public ICommand AddListCommand { get; }
    public Action? OnSaveAddList { get; set; }
    private bool _toggleArchive = false;
    public bool ToggleArchive
    {
        get => _toggleArchive;
        set
        {
            _toggleArchive = value;
            if (_toggleArchive) FilteredLists = _store.Archive.ArchivedLists;
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

    public GroupListViewModel(        
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate):
        base(store, navigator, dialogService, stateService, notificate)
    {
        FilteredLists = _store.FilteredLists;
        _store.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Store.FilteredLists) && !_toggleArchive)
            {
                FilteredLists = _store.FilteredLists;
                OnPropertyChanged(nameof(FilteredLists));
            }
        };

        OpenListCommand = new RelayCommand<string>(async (listName) =>
        {
            var list = _store.Lists.FirstOrDefault(l => l.ListName == listName);
            if (list != null)
                await OpenListAsync(list);
            ClearSelectedList();
        });
        AddListCommand = new AsyncRelayCommand<string>(AddList);
        if (_stateService is ChangeStateService service) service.SelectedListCleared += ClearSelectedList;

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
        _store.SelectedList = groupedList;
        _store.SelectedListName = groupedList?.ListName;
        _navigator.ClearStack();
        
        var vm = App.Services?.GetRequiredService<TaskGroupViewModel>();
        await _navigator.NavigateRight((vm, new Components.TopBarViewModel(_store, vm)));
        _stateService.OpenPane(false);
    }

    private void ClearSelectedList()
    {
        _selectedList = null;
        OnPropertyChanged(nameof(SelectedList));
    }
}