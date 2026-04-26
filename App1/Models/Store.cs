using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_EventHub;
using App1.Events;
using App1.ViewModels;

public class Store : ModelBase
{
    private StoreHelpers _helper;

    public string? SelectedListName { get; private set; }
    public GroupList? SelectedList { get; private set; }
    public BaseTask? SelectedTask { get; set; }

    public string WelcomeText { get; set; } = "Welcome";
    public string? TopbarText { get; set; }
        
    public ArchivedList ArchiveLists { get; set; }
    public required MainList MainLists { get; set; }

    public ObservableCollection<TaskGroup>? ImportantLists { get; set; }
    public ObservableCollection<GroupList> FilteredLists { set; get; } = new();


    public Store(IEventHub events) : base (events)
    {
        MainLists = new MainList(_events);
        ArchiveLists = new ArchivedList(_events);

        _helper = new(this, _events);
    }


    public void StoreUpdateImportantList()
    {
        ImportantLists = _helper.FilterImportant();
        _events.Publish(new ImportantListChangedEvent(ImportantLists));
    }
    public void StoreUpdateFilteredLists()
    {
        FilteredLists = new ObservableCollection<GroupList>(
            MainLists.MainLists.Where(g => g.ListName != GlobalVariables.Quick)
        );
        _events.Publish(new FilteredListsChangedEvent(FilteredLists));
    }

    public void SelectList(GroupList? list)
    {
        SelectedList = list;
        SelectedListName = list?.ListName;
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
    public void SelectListName(string? text)
    {
        if (text == null) return;
        SelectedListName = text;
        _events.Publish(new SelectedListNameChangedEvent(text));
    }

    public ObservableCollection<GroupList> CreateDefaultList() => new()
    {
        new GroupList(_events)
        {
            ListName = "Quick",
            Groups = []
        }
    };


    public bool StoreAddList(string name)
    {
        var isExisted = _helper.AddList(name);
        if (!isExisted) 
        {
            _events.Publish(new MainListsChangedEvent(MainLists));
            StoreUpdateFilteredLists();
            return false;
        }
        return true;
    }
    //GroupListChangedEvent
    //TaskGroupChangedEvent
    public void StoreAddTaskToCategory(BaseTask task)
    {
        _helper.AddTaskToCategory(task);
    }


    //GroupListChangedEvent
    //TaskGroupChangedEvent
    public void StoreDeleteTask(BaseTask task, bool isMainList)
    {
        _helper.DeleteTask(task, isMainList);
    }

    public void StoreDeleteList(string list, bool isMainList)
    {
        _helper.DeleteList(list, isMainList);
        if (isMainList) 
        {
            _events.Publish(new MainListsChangedEvent(MainLists));
            StoreUpdateFilteredLists();
        }
        else  _events.Publish(new ArchiveListsChangedEvent(ArchiveLists));
        SelectList(null);
    }


    public void StoreMoveToArchive(string list)
    {
        _helper.MoveToArchive(list);
        _events.Publish(new MainListsChangedEvent(MainLists));
        _events.Publish(new ArchiveListsChangedEvent(ArchiveLists));
        StoreUpdateFilteredLists();    
    }   
    public void StoreMoveToList(string list)
    {
        _helper.MoveToList(list);
        _events.Publish(new MainListsChangedEvent(MainLists));
        _events.Publish(new ArchiveListsChangedEvent(ArchiveLists));
        StoreUpdateFilteredLists();  
    }


    public void StoreEditList(
        string oldListName,
        string newListName,
        ObservableCollection<TaskGroup>? editedGroups
    ){
        SelectedListName = newListName;
        _helper.EditList(oldListName, newListName, editedGroups);
    }    
}