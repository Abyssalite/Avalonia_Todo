using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected readonly Store _store;
    protected readonly INavigatorService _navigator;
    protected readonly IDialogService _dialogService;
    protected readonly IChangeStateService _stateService;
    public INotificationService Notificate { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public AsyncRelayCommand ToggleArchiveCommand { get; }
    public AsyncRelayCommand BackOrDrawerCommand { get; }
    public RelayCommand EditCommand { get; }
    public Action<bool>? OnChangeListName { get; set; }

    protected ViewModelBase(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate)
    {
        _store = store;
        _navigator = navigator;
        _dialogService = dialogService;
        _stateService = stateService;
        Notificate = notificate;

        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        ToggleArchiveCommand = new AsyncRelayCommand(ToggleArchiveListAsync);
        BackOrDrawerCommand = new AsyncRelayCommand(BackOrToggleDrawerAsync);
        EditCommand = new RelayCommand(Edit);
    }
    protected virtual Task DeleteAsync() => Task.CompletedTask;
    protected virtual Task ToggleArchiveListAsync() => Task.CompletedTask;
    protected virtual Task BackOrToggleDrawerAsync() => Task.CompletedTask;
    protected virtual void Edit() { }
    public virtual bool? GetSetImportant(bool? value) { return null; }
}
