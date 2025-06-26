using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public partial class TaskDetailViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    private ViewModelBase _viewModel;
    private readonly IDialogHelper _dialogHelper;
    public BaseTask? Task { get; }
    public ICommand ShowDialogCommand { get; }
    public ICommand BackCommand { get; }

    public TaskDetailViewModel(IViewHost host, ViewModelBase viewModel, Store store)
    {
        _store = store;
        _host = host;
        _dialogHelper = new DialogHelper();
        _viewModel = viewModel;
        if (store.SelectedTask != null)
            Task = store.SelectedTask;

        ShowDialogCommand = new AsyncRelayCommand(OnShowDialogAsync);
        BackCommand = new RelayCommand(async () => await host.NavigateRight(_viewModel));
    }

    public async Task DeleteTaskAsync()
    {
        if (Task != null)
        {
            await TaskHelpers.DeleteTask(Task, _store);
            await _host.NavigateRight(_viewModel);
        }
    }
    
    private async Task OnShowDialogAsync()
    {
        bool? confirmed = await _dialogHelper.ShowDialogAsync("Do you want to Delete?");
        if(confirmed == true) await DeleteTaskAsync();
    }
}
