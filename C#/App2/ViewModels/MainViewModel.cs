using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using App2.Views;
using System.Linq;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public TaskGroupViewModel? TaskGroupView { get; private set; }
    private ObservableCollection<GroupList> _groupedList = new();
    public ObservableCollection<GroupList> FilteredGroupedList { get; set; }
    public ICommand AddQuickTaskCommand { get; } //Button only
    //public ICommand AddGroupDialogCommand { get; } //Button only
    private GroupList? _selectedGroup;
    public GroupList ? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (value != null && value != _selectedGroup)
            {
                _selectedGroup = value;
                OpenGroup(value);
                OnPropertyChanged(nameof(SelectedGroup));

                SelectedGroup = null; // reset to allow re-selection
            }
        }
    }

    public MainViewModel(MainWindowViewModel main)
    {
        _mainWindowViewModel = main;
        _groupedList = TaskHelpers.Load();

        HookSaveOnIsDoneChange();
        AddQuickTaskCommand = new RelayCommand<string>(OpenAddTaskView);
        FilteredGroupedList = new ObservableCollection<GroupList>(_groupedList.Where(g => g.List != "Quick"));
        //AddGroupDialogCommand = new RelayCommand(OpenAddTaskView);
    }

    private void HookSaveOnIsDoneChange()
    {
        foreach (var list in _groupedList)
        foreach (var group in list.Groups)
        foreach (var task in group.Tasks)
            TaskHelpers.HookSaveToTask(_groupedList, task);
    }

    private void OpenAddTaskView(string? listName)
    {
        if (listName != null)
        {
            var addVM = new AddTaskViewModel(_mainWindowViewModel, listName);
            addVM.OnTaskCreated = task =>
            {
                TaskHelpers.AddTaskToCategory(task, _groupedList);
            };
            _mainWindowViewModel.MainView = addVM;  
        }
    }

    private void OpenGroup(GroupList list)
    {
        TaskGroupView = new TaskGroupViewModel(_mainWindowViewModel, list.Groups);
        TaskGroupView.OnTaskCreate = OpenAddTaskView;
        TaskGroupView.OnTaskDetele = task =>
        {
            TaskHelpers.DeleteTask(task, _groupedList);
        };
        OnPropertyChanged(nameof(TaskGroupView));
    }
}
