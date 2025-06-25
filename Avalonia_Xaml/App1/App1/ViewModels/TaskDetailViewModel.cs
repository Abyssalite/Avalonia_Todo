using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace App1.ViewModels;

public class TaskDetailViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    private ViewModelBase _viewModel;
    public BaseTask? Task { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand BackCommand { get; }
    public Action? ShowDeleteDialog { get; set; }

    public TaskDetailViewModel(IViewHost host, ViewModelBase viewModel, Store store)
    {
        _store = store;
        _host = host;
        _viewModel = viewModel;
        if (store.SelectedTask != null)
            Task = store.SelectedTask;

        DeleteTaskCommand = new RelayCommand(DeleteTask);
        BackCommand = new RelayCommand(() =>  _host.NavigateRight(_viewModel));
    }

    public void DeleteTask()
    {
        if (Task != null)
        {
            TaskHelpers.DeleteTask(Task, _store);
            _host.NavigateRight(_viewModel);
        }
    }
}
