using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia_Navigation;
using Avalonia_EventHub;
using System.Collections.Generic;

namespace App1.ViewModels;

public partial class ViewModelBase : ObservableObject, IDisposable
{
    protected readonly List<IDisposable> _subscriptions = new();

    protected readonly Store _store;
    protected readonly INavigatorService _navigator;
    protected readonly IDialogService _dialogService;
    protected readonly IChangeStateService _stateService;
    protected readonly IEventHub _events;

    public INotificationService Notificate { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public AsyncRelayCommand SetArchiveCommand { get; }
    public AsyncRelayCommand BackOrDrawerCommand { get; }
    public RelayCommand EditCommand { get; }

    protected ViewModelBase(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate,
        IEventHub events
    ){
        _store = store;
        _navigator = navigator;
        _dialogService = dialogService;
        _stateService = stateService;
        _events = events;
        Notificate = notificate;

        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        SetArchiveCommand = new AsyncRelayCommand(SetArchiveList);
        BackOrDrawerCommand = new AsyncRelayCommand(BackOrToggleDrawerAsync);
        EditCommand = new RelayCommand(Edit);
    }
    protected virtual async Task DeleteAsync() => await Task.CompletedTask;
    protected virtual async Task SetArchiveList() => await Task.CompletedTask;
    protected virtual async Task BackOrToggleDrawerAsync()
    {
        if (GlobalVariables.IsAndroid)
            _stateService.OpenPane(!_stateService.IsPaneOpen);
        else
            await _navigator.OpenPrevious();

        await Task.CompletedTask;
    }
    
    protected virtual void Edit() {}
    public virtual bool GetIsImportant() => false;

    public void Dispose()
    {
        foreach (var sub in _subscriptions)
            sub.Dispose();
    }
}
