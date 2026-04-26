using System;
using System.Collections.ObjectModel;
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
    }    private GroupList? _selectedList;
    public GroupList? SelectedList
    {
        get => _selectedList;
        set
        {
            if (value == null || _selectedList == value) return;

            _selectedList = value;
            _ = OpenListAsync(_selectedList);
            OnPropertyChanged(nameof(SelectedList));
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
        DisplayLists = _store.FilteredLists;

        _subscriptions.Add(_events.Subscribe<FilteredListsChangedEvent>(evt =>
        {
            if (!ToggleArchive)
            {
                DisplayLists = evt.Lists;
                OnPropertyChanged(nameof(DisplayLists));
            }
        }));
        _subscriptions.Add(_events.Subscribe<ArchiveListsChangedEvent>(evt =>
        {
            if (ToggleArchive)
            {
                DisplayLists = evt.Lists.ArchivedLists;
                OnPropertyChanged(nameof(DisplayLists));
            }
        }));
        _subscriptions.Add(_events.Subscribe<SelectedListChangedEvent>(evt =>
        {
                _selectedList = evt.SelectedList;
                OnPropertyChanged(nameof(SelectedList));
        }));


        OpenListCommand = new RelayCommand<string>(async (listName) =>
        {
            if (TaskHelpers.IsQuickList(listName))
            {
                var list = _store.QuickList;
                if (list != null)
                    await OpenListAsync(list);
            }
            else if (listName == GlobalVariables.Important)
            {
                var list = new GroupList(_events)
                {
                    ListName = listName,
                    Groups = _store.ImportantLists ?? []
                };
                await OpenListAsync(list);
            }
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
            bool isExisted = await _store.StoreAddListAsync(newListName);
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
        _store.SelectList(null);
        await Task.CompletedTask;
    }
}