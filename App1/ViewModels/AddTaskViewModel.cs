using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Avalonia_Navigation;

namespace App1.ViewModels;

public partial class AddTaskViewModel : ViewModelBase
{
    public ICommand SaveTaskCommand { get; }
    public ICommand CancelCommand { get; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
    public bool _isImportant;
    private string _listName;


    public AddTaskViewModel(
        Store store,
        INavigatorService navigator,
        IDialogService dialogService,
        IChangeStateService stateService,
        INotificationService notificate
    ): base(store, navigator, dialogService, stateService, notificate)
    {
        SaveTaskCommand = new AsyncRelayCommand(AddTask);
        CancelCommand = new AsyncRelayCommand(ClearAsync);
        _listName = _store.SelectedListName;
        _isImportant = _listName == GlobalVariables.Important;
    }

    public override bool? GetSetImportant(bool? value)
    {
        if (value == null) return _isImportant;
        else if (value == true)
        {
            _isImportant = true;
        }
        else if (value == false)
        {
            _isImportant = false;
        } 
        return null;
    }

    private async Task ClearAsync()
    {
        NewTaskName = string.Empty;
        OnPropertyChanged(nameof(NewTaskName));
        TaskCatalog = string.Empty;
        OnPropertyChanged(nameof(TaskCatalog));
        TaskDesc = string.Empty;
        OnPropertyChanged(nameof(TaskDesc));
        await _navigator.OpenPrevious();
    }

    private async Task AddTask()
    {
        string name = TaskHelpers.InputOrDefault(NewTaskName, "");
        if (name == "")
        {
            Notificate.ShowNotification("Task name cannot be Empty!");
            return;
        }
        var task = new BaseTask
        {
            Name = name,
            IsDone = false,
            ListName = (_listName == GlobalVariables.Important) ? GlobalVariables.Quick : _listName,
            Category = TaskHelpers.InputOrDefault(TaskCatalog, "Miscelanious"),
            Description = TaskHelpers.InputOrDefault(TaskDesc, ""),
            IsImportant = _isImportant
        };
        await TaskHelpers.AddTaskToCategory(task, _store);
        _stateService.UpdateImportant();
        await ClearAsync();
    }
}
