using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Ursa.Controls;

namespace App1.ViewModels;

public class TaskGroupViewModel : ViewModelBase
{
    private Store _store { get; }
    private readonly IViewHost _host;
    public ObservableCollection<TaskGroup>? GroupedTasks { get; }
    public string? ListName { get; }
    public ICommand AddTaskViewCommand { get; }
    public ICommand DeleteListCommand { get; }
    public ICommand OkCancelCommand { get; set; }
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

    private string _message = "Hello";
    private string? _title = "Hello";
    private MessageBoxIcon _selectedIcon = MessageBoxIcon.Question;
    private MessageBoxResult _result;
    public MessageBoxResult Result
    {
        get => _result;
        set => SetProperty(ref _result, value);
    }

    public TaskGroupViewModel(IViewHost host, Store store)
    {
        _store = store;
        _host = host;
        if (store.SelectedList != null)
        {
            ListName = store.ListName;
            GroupedTasks = store.SelectedList.Groups;
        }

        AddTaskViewCommand = new AsyncRelayCommand(OpenAddTaskView);
        DeleteListCommand = new AsyncRelayCommand(DeleteList);
        OkCancelCommand = new AsyncRelayCommand(OnOkCancelAsync);
    }
    
        private async Task DeleteList()
    {
        if (ListName != null && ListName != "Quick")
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
        await _host.NavigateRight(new TaskDetailViewModel(_host,_host.RightView, _store));
    } 

    private async Task OnOkCancelAsync()
    {
        await Show(MessageBoxButton.OKCancel);
    }
    
    private async Task Show(MessageBoxButton button)
    {
         Result = await MessageBox.ShowAsync(_message, _title, icon: _selectedIcon, button: button);
    }
}
