using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace App1.ViewModels;

public partial class TaskDetailViewModel : ViewModelBase
{
    private Store _store;
    private IViewHost _host;
    private readonly IDialogService _dialogService;
    public BaseTask? Task { get; }
    public ICommand ShowDialogCommand { get; }
    public ICommand BackCommand { get; }

    public TaskDetailViewModel(IViewHost host, Store store, IDialogService dialogService)
    {
        _store = store;
        _host = host;
        _dialogService = dialogService;
        if (store.SelectedTask != null)
            Task = store.SelectedTask;

        ShowDialogCommand = new AsyncRelayCommand<object>(OnShowDialogAsync);
        BackCommand = new RelayCommand(async () => await _host.NavigateRight(App.Services?.GetRequiredService<TaskGroupViewModel>()));
    }

    public async Task DeleteTaskAsync()
    {
        if (Task != null)
        {
            await TaskHelpers.DeleteTask(Task, _store);
            await _host.NavigateRight(App.Services?.GetRequiredService<TaskGroupViewModel>());
        }
    }
    
    private async Task OnShowDialogAsync(object? parameter)
    {
        if (parameter is Button button)
        {
            button.Flyout?.Hide();
        }
        bool? confirmed = await _dialogService.ShowDialogAsync("Do you want to Delete?", null);
        if(confirmed == true) await DeleteTaskAsync();
    }
}
