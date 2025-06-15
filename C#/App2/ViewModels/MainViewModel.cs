using System.Collections.ObjectModel;
using App2.Views;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace App2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public TaskGroupViewModel? TaskGroupView { get; private set; }
    public ObservableCollection<GroupList> GroupedList { get; set; }
    //public ICommand AddGroupDialogCommand { get; } //Button only
    private GroupList ? _selectedGroup;
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
        GroupedList = TaskHelpers.Load();
        HookSaveOnIsDoneChange();
        //AddGroupDialogCommand = new RelayCommand(OpenAddTaskView);
    }

    private void HookSaveOnIsDoneChange()
    {
        foreach (var list in GroupedList)
        foreach (var group in list.Groups)
        foreach (var task in group.Tasks)
            TaskHelpers.HookSaveToTask(GroupedList, task);
    }

    private void OpenAddTaskView()
    {
        var addVM = new AddTaskViewModel(_mainWindowViewModel);
        addVM.OnTaskCreated = task =>
        {
            
        };
        _mainWindowViewModel.MainView = addVM;
    }

    private void OpenGroup(GroupList  list)
    {
        TaskGroupView = new TaskGroupViewModel(_mainWindowViewModel, list.Groups);
        TaskGroupView.OnTaskDetele = task =>
        {
            TaskHelpers.DeleteTask(task, GroupedList);
        };
        OnPropertyChanged(nameof(TaskGroupView));
    }
}
