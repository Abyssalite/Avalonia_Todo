using System.Threading.Tasks;
using System.Windows.Input;
using App1.Components;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public partial class TaskDetailViewModel : ViewModelBase
{
    private readonly Store _store;
    public readonly INavigatorService _navigator;
    private readonly IDialogService _dialogService;
    public BaseTask? Task { get; }
    public bool IsNotInArchive { get; } = true;
    public ICommand ShowDialogCommand { get; }
    public ICommand BackCommand { get; }

    public TaskDetailViewModel(Store store, INavigatorService navigator, IDialogService dialogService)
    {
        _store = store;
        _navigator = navigator;
        _dialogService = dialogService;
        if (store.SelectedTask != null && store.SelectedList != null)
        {
            Task = store.SelectedTask;
            IsNotInArchive = !store.SelectedList.IsArchived;
        }

        ShowDialogCommand = new AsyncRelayCommand<object>(OnShowDialogAsync);
        BackCommand = new RelayCommand(async () => await _navigator.OpenPrevious());
    }
    
    private async Task OnShowDialogAsync(object? parameter)
    {
        if (parameter is Button button)
            button.Flyout?.Hide();

        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?");
        if (confirmed == true && Task != null)
        {
            await TaskHelpers.DeleteTask(Task, _store, !IsNotInArchive);
            await _navigator.OpenPrevious(); 
        }
    }
}
