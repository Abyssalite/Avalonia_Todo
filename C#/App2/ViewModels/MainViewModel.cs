using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using App2.Views;
using System.Linq;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public TaskDetailViewModel? TaskDetailView { get; private set; }
    public ObservableCollection<GroupList> GroupLists { get; set; }
    public string? NewTaskName { get; set; }
    public string? TaskDesc { get; set; }
    public string? TaskCatalog { get; set; }
    public ICommand AddTaskViewCommand { get; } //Button only
    private BaseTask? _selectedTask;
    public BaseTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (value != null && value != _selectedTask)
            {
                _selectedTask = value;
                OpenTask(value);
                OnPropertyChanged(nameof(SelectedTask));

                SelectedTask = null; // reset to allow re-selection
            }
        }
    }

    public MainViewModel(MainWindowViewModel main)
    {
        _mainWindowViewModel = main;
        GroupLists = TaskHelpers.Load();
        HookSaveOnIsDoneChange();
        AddTaskViewCommand = new RelayCommand(OpenAddTaskView);
    }

    public void AddTaskToCategory(BaseTask task)
    {
        var list = GroupLists.FirstOrDefault(l => l.List == task.List);
        if (list == null) return;

        var group = list.Group.FirstOrDefault(g => g.Category == task.Category);
        if (group != null)
        {
            group.Tasks.Add(task);
            return;
        }

        // Create new category group if it doesn't exist
        list.Group.Add(new TaskGroup
        {
            Category = task.Category,
            Tasks = new ObservableCollection<BaseTask> { task }
        });
    }

    public void DeleteTask(BaseTask task)
    {
        var list = GroupLists.FirstOrDefault(l => l.List == task.List);
        if (list == null) return;

        var group = list.Group.FirstOrDefault(g => g.Category == task.Category);
        if (group == null) return;

        group.Tasks.Remove(task);

        if (group.Tasks.Count == 0)
        {
            list.Group.Remove(group);
        }
    }

    private void HookSaveOnIsDoneChange()
    {
        foreach (var list in GroupLists)
        foreach (var group in list.Group)
        foreach (var task in group.Tasks)
            TaskHelpers.HookSaveToTask(GroupLists, task);
    }

    private void OpenAddTaskView()
    {
        var addVM = new AddTaskViewModel(_mainWindowViewModel);
        addVM.OnTaskCreated = task =>
        {
            AddTaskToCategory(task);
            TaskHelpers.Save(GroupLists);
        };
        _mainWindowViewModel.MainView = addVM;
    }

    private void OpenTask(BaseTask task)
    {
        TaskDetailView = new TaskDetailViewModel(_mainWindowViewModel,task);
        TaskDetailView.OnTaskDetele = task =>
        {
            DeleteTask(task);
            TaskHelpers.Save(GroupLists);

        };
        OnPropertyChanged(nameof(TaskDetailView));
    }
}
