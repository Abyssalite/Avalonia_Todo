using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_EventHub;
using App1.Events;
using System;
using App1.ViewModels;
using System.Threading.Tasks;

public class Store : ModelBase
{
    private ObservableCollection<GroupList> _lists = new();
    private StoreHelpers _helper;

    public string? SelectedListName { get; private set; }
    public GroupList? SelectedList { get; private set; }
    public BaseTask? SelectedTask { get; set; }
    public ArchivedList ArchiveLists { get; set; }
    public string WelcomeText { get; set; } = "Welcome";
    public bool Initialized { get; set; }
    public required MainList MainLists { get; set; }
    public string? TopbarText { get; set; }

    public ObservableCollection<TaskGroup>? ImportantList { get; set; }


    public Store(IEventHub events) : base (events)
    {
        MainLists = new MainList(_events);
        ArchiveLists = new ArchivedList(_events);

        _helper = new(this, _events);
    }

    public ObservableCollection<GroupList> FilteredLists =>
        new(_lists.Where(g => g.ListName != GlobalVariables.Quick));



    public void SelectList(GroupList? list)
    {
        if (list == null) return;
        SelectedList = list;
        SelectedListName = list.ListName;
        _events.Publish(new SelectedListChangedEvent(SelectedList, SelectedListName));
    }
    public void SelectTask(BaseTask? task)
    {
        if (task == null) return;
        SelectedTask = task;
        _events.Publish(new SelectedTaskChangedEvent(SelectedTask));
    }

    public void SetTaskImportant(bool? value)
    {
        if (SelectedTask == null) return;
        SelectedTask.IsImportant = value;
    }
    public void SetTaskDone(bool? value)
    {
        if (SelectedTask == null) return;
        SelectedTask.IsDone = value;
    }

    public void OnChangeListName(bool value)
    {
        _events.Publish(new ChangeListNameEvent(value));
    }
    public void EditTopBarText(string? text)
    {
        if (text == null) return;
        TopbarText = text;
        _events.Publish(new TopbarTextChangedEvent(text));
    }

    public void StoreAddTaskToCategory(BaseTask task)
    {
        _helper.AddTaskToCategory(task);
    }
    public bool StoreAddList(string name)
    {
        return _helper.AddList(name);
    }


    public void StoreUpdateImportantList()
    {
        if (SelectedListName == null) return;
        ImportantList = _helper.FilterImportant();
        _events.Publish(new ChangeImportantListEvent());
    }


    public void StoreDeleteTask(BaseTask task, bool isMainList)
    {
        _helper.DeleteTask(task, isMainList);
    }
    public void StoreDeleteList(string list, bool isMainList)
    {
        _helper.DeleteList(list, isMainList);
    }


    public void StoreMoveToArchive(string list)
    {
        _helper.MoveToArchive(list);
    }   
    public void StoreMoveToList(string list)
    {
        _helper.MoveToList(list);
    }


    public void StoreEditList(
        string oldListName,
        string newListName,
        ObservableCollection<TaskGroup>? editedGroups
    ){
        _helper.EditList(oldListName, newListName, editedGroups);
    }    
}