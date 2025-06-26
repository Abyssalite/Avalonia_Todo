using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;
using App1.Dialogs;

namespace App1.ViewModels;

public partial class TaskGroupViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    private readonly IDialogHelper _dialogHelper;
    public ObservableCollection<TaskGroup>? GroupedTasks { get; }
    public string? ListName { get; }
    public ICommand AddTaskViewCommand { get; }
    public ICommand ShowDialogCommand { get; }
    private BaseTask? _selectedTask;
    public BaseTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (value != null && value != _selectedTask)
            {
                _selectedTask = value;
                _ = OpenTaskAsync(value);
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
    }

    public TaskGroupViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
        _dialogHelper = new DialogHelper();
        if (store.SelectedList != null)
        {
            ListName = store.ListName;
            GroupedTasks = store.SelectedList.Groups;
        }

        AddTaskViewCommand = new AsyncRelayCommand(OpenAddTaskView);
        ShowDialogCommand = new AsyncRelayCommand(OnShowDialogAsync);
    }

    private async Task DeleteList()
    {
        if (ListName != null)
        {
            await TaskHelpers.DeleteList(ListName, _store);
            await _host.NavigateRight(new WellcomeViewModel("Wellcome"));
        }
    }

    private async Task OpenAddTaskView()
    {
        if (ListName != null)
        {
            await _host.NavigateRight(new AddTaskViewModel(_host, _host.RightView, _store));
            await _host.NavigateLeft(new NewTaskOptionViewModel(_host, _store));
        }
    }

    private async Task OpenTaskAsync(BaseTask task)
    {
        _selectedTask = null;
        OnPropertyChanged(nameof(SelectedTask));

        _store.SelectedTask = task;
        await _host.NavigateRight(new TaskDetailViewModel(_host, _host.RightView, _store));
    }

    private async Task OnShowDialogAsync()
    {
        if (ListName == "Quick") return;
        bool? confirmed = await _dialogHelper.ShowDialogAsync("Do you want to Delete?");
        if(confirmed == true) await DeleteList();
    }
}
