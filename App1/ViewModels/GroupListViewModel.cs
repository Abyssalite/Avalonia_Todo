using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Avalonia_Navigation;
using Avalonia_EventHub;
using App1.Events;

namespace App1.ViewModels;

public partial class GroupListViewModel : ViewModelBase, IHandleLastPage, IDisposable
{
    public ObservableCollection<GroupList>? DisplayLists { get; set; } = new();
    public ICommand? OpenListCommand { get; }
    public ICommand? AddListCommand { get; }
    public Action? OnSaveAddList { get; set; }
    private bool _toggleArchive = false;
    public bool ToggleArchive
    {
        get => _toggleArchive;
        set
        {
            _toggleArchive = value;
            if (_toggleArchive) DisplayLists = _store.ArchiveLists.ArchivedLists;
            else DisplayLists = _store.FilteredLists;

            OnPropertyChanged(nameof(DisplayLists));
            OnPropertyChanged(nameof(ToggleArchive));
        }
    }
    private GroupList? _selectedList;
    public GroupList? SelectedList
    {
        get => _selectedList;
        set
        {
            if (value != null && _selectedList != value)
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
        INotificationService notificate,
        IEventHub events
    ): base(store, navigator, dialogService, stateService, notificate, events)
    {
        if (_store.FilteredLists == null) return;

        DisplayLists = _store.FilteredLists;

        _subscriptions.Add(_events.Subscribe<FilteredListsChangedEvent>(_ =>
        {
            if (_toggleArchive)
            {
                DisplayLists = _store.FilteredLists;
                OnPropertyChanged(nameof(DisplayLists));
            }
        }));

        _subscriptions.Add(_events.Subscribe<ArchiveListsChangedEvent>(_ =>
        {
            if (_toggleArchive)
            {
                DisplayLists = _store.ArchiveLists.ArchivedLists;
                OnPropertyChanged(nameof(DisplayLists));
            }
        }));


        OpenListCommand = new RelayCommand<string>(async (listName) =>
        {
            if (TaskHelpers.IsMainList(listName))
            {
                var list = _store.MainLists.MainLists.FirstOrDefault(l => l.ListName == listName);
                if (list != null)
                    await OpenListAsync(list);
            }
            else if (listName == GlobalVariables.Important)
            {
                var list = new GroupList(_events)
                {
                    ListName = listName,
                    Groups = _store.ImportantList ?? []
                };
                await OpenListAsync(list);
            }
            ClearSelectedList();
        });

        AddListCommand = new RelayCommand<string>(AddList);
        _stateService.ClearSelectedListAction += ClearSelectedList;
    }

    private void AddList(string? newListName)
    {
        OnSaveAddList?.Invoke();
        if (newListName != null)
        {
            bool isExisted = _store.StoreAddList(newListName);
            if (isExisted) Notificate.ShowNotification("The list is Existed");
        }
    }

    private async Task OpenListAsync(GroupList groupedList)
    {
        _store.SelectList(groupedList);
        await _navigator.ClearStack();

        _stateService.CancelEdit();
        _stateService.OpenPane(false);
        var vm = App.Services?.GetRequiredService<TaskGroupViewModel>();
        await _navigator.NavigateMainAndTop(vm, new Components.TopBarViewModel(_store, vm, _events, groupedList.ListName));
    }

    async Task IHandleLastPage.HandleLastPageAsync()
    {
        ClearSelectedList();
        await Task.CompletedTask;
    }

    private void ClearSelectedList()
    {
        _selectedList = null;
        //OnPropertyChanged(nameof(SelectedList));
    }
}