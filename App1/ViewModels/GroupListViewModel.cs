using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Avalonia_Navigation;

namespace App1.ViewModels;

public partial class GroupListViewModel : ViewModelBase, IHandleLastPage
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
        INotificationService notificate
    ): base(store, navigator, dialogService, stateService, notificate)
    {
        FilteredLists = _store.FilteredLists;
        _store.PropertyChanged += (_, e) =>
        {
            // Update GUI after add new List
            if (e.PropertyName == nameof(Store.FilteredLists) && !_toggleArchive)
            {
                FilteredLists = _store.FilteredLists;
                OnPropertyChanged(nameof(FilteredLists));
            }
        };

        OpenListCommand = new RelayCommand<string>(async (listName) =>
        {
            if (TaskHelpers.IsMainList(listName))
            {
                var list = _store.Lists.FirstOrDefault(l => l.ListName == listName);
                if (list != null)
                    await OpenListAsync(list);
            }
            else if (listName == GlobalVariables.Important)
            {
                var list = new GroupList()
                {
                    ListName = listName,
                    Groups = TaskHelpers.FilterImportant(_store.Lists)
                };
                await OpenListAsync(list);
            }
            ClearSelectedList();
        });
        AddListCommand = new AsyncRelayCommand<string>(AddList);
        _stateService.ClearSelectedListAction += ClearSelectedList;
    }

    async Task IHandleLastPage.HandleLastPageAsync()
    {
        _stateService.ClearSelectedList();
        await Task.CompletedTask;
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
        _store.SelectedListName = groupedList.ListName;
        await _navigator.ClearStack();

        _stateService.CancelEdit();
        _stateService.OpenPane(false);
        var vm = App.Services?.GetRequiredService<TaskGroupViewModel>();
        await _navigator.NavigateMainAndTop(vm, new Components.TopBarViewModel(_store, vm, groupedList.ListName));
    }

    private void ClearSelectedList()
    {
        _selectedList = null;
        OnPropertyChanged(nameof(SelectedList));
    }
}