using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using App2.Views;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public TaskGroupViewModel? TaskGroupView { get; private set; }
    private ViewModelBase _sideView;
    public ViewModelBase SideView
    {
        get => _sideView;
        set => SetProperty(ref _sideView, value);
    }
    public ICommand OpenQuickTaskCommand { get; } //Button only
    //public ICommand AddGroupDialogCommand { get; } //Button only
    public ObservableCollection<GroupList> FilteredGroupedList { get; set; } = new();
    private ObservableCollection<GroupList> _groupedList = new();
    public ObservableCollection<GroupList> GroupedList
    {
        get => _groupedList;
        set
        {
            if (value != null)
            {
                _groupedList = value;
                FilteredGroupedList = new ObservableCollection<GroupList>(_groupedList.Where(g => g.List != "Quick"));
            }
        }
    }
    private GroupList? _selectedGroup;
    public GroupList ? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (value != null && value != _selectedGroup)
            {
                _selectedGroup = value;
                OpenGroup(_selectedGroup, _selectedGroup.List);
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }
    }

    public MainViewModel(MainWindowViewModel main)
    {
        _mainWindowViewModel = main;
        GroupedList = TaskHelpers.Load();
        HookSaveOnIsDoneChange();

        _sideView = new WellcomeViewModel(this, "Wellcome");
        OpenQuickTaskCommand = new RelayCommand(() =>
        {
            var quick = _groupedList.FirstOrDefault(l => l.List == "Quick");
            if (quick != null)
                OpenGroup(quick, "Quick");
                _selectedGroup = null;
                OnPropertyChanged(nameof(SelectedGroup));
        });
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

    private void OpenGroup(GroupList list, string listName)
    {
        TaskGroupView = new TaskGroupViewModel(this, list.Groups, listName);
        TaskGroupView.OnTaskCreate = OpenAddTaskView;
        TaskGroupView.OnTaskDetele = task =>
        {
            _groupedList = TaskHelpers.DeleteTask(task, _groupedList);

        };
        SideView = TaskGroupView;
    }
}
