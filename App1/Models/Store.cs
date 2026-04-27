using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_EventHub;
using App1.Events;
using System.Threading.Tasks;

public class Store
{
    private StoreHelpers _helper;
    private readonly IEventHub _events;

    public string? SelectedListName { get; private set; }
    public GroupList? SelectedList { get; private set; }
    public BaseTask? SelectedTask { get; private set; }

    public string WelcomeText { get; set; } = "Welcome";
    public string? TopbarText { get; set; }
        
    public ArchivedList ArchiveLists { get; private set; }
    public required MainList MainLists { get; set; }

    public ObservableCollection<TaskGroup>? ImportantLists { get; private set; }
    public ObservableCollection<GroupList> FilteredLists { get; private set; } = new();
    public GroupList? QuickList { get; private set; }


    public Store(IEventHub events)
    {
        _events = events;
        MainLists = new MainList();
        ArchiveLists = new ArchivedList();

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
    public void StoreUpdateQuickList()
    {
        QuickList = MainLists.MainLists.FirstOrDefault(g => g.ListName == GlobalVariables.Quick);
        _events.Publish(new QuickListsChangedEvent(QuickList));
    }


    public void SelectList(GroupList? list)
    {
        if (SelectedList == list) return;

        SelectedList = list;
        SelectedListName = list?.ListName;
        
        SelectTask(null);
        TopbarText = SelectedListName;
        _events.Publish(new SelectedListChangedEvent(SelectedList, SelectedListName));
    }
    public void SelectTask(BaseTask? task)
    {
        if (SelectedTask == task) return;

        SelectedTask = task;
        _events.Publish(new SelectedTaskChangedEvent(SelectedTask));
    }
    

    public void SetTaskImportant(bool isImportant)
    {
        if (SelectedTask == null) {
            _events.Publish(new TaskIsImportantChangedEvent(null, isImportant));
            return;
        }
        SelectedTask.IsImportant = isImportant;
        _events.Publish(new TaskIsImportantChangedEvent(SelectedTask, isImportant));

        StoreUpdateImportantList();
    }
    public void SetTaskDone(bool? isDone)
    {
        if (SelectedTask == null) return;
        SelectedTask.IsDone = isDone;
        _events.Publish(new TaskIsDoneChangedEvent(SelectedTask, isDone));
    }

    
    public void OnEnterEdit(bool isEdit, string name)
    {
        _events.Publish(new EnterEditModeEvent(isEdit, name));
    }
    public void SetTopBarText(string? text)
    {
        if (text == null) return;
        TopbarText = text;
        _events.Publish(new TopbarTextChangedEvent(text));
    }


    public ObservableCollection<GroupList> CreateDefaultList() => new()
    {
        new GroupList()
        {
            ListName = "Quick",
            Groups = []
        }
    };


    public async Task<bool> StoreAddListAsync(string name)
    {
        var isExisted = _helper.AddList(name);
        if (!isExisted) 
        {
            _events.Publish(new MainListsChangedEvent(MainLists));
            StoreUpdateFilteredLists();

            await TaskHelpers.SaveAsync(this, true);            
            return false;
        }
        return true;
    }
    //GroupListChangedEvent
    //TaskGroupChangedEvent
    public async Task StoreAddTaskToCategoryAsync(BaseTask task, bool isImportant)
    {
        _helper.AddTaskToCategory(task);

        if (isImportant) StoreUpdateImportantList();
        else StoreUpdateQuickList();

        await TaskHelpers.SaveAsync(this, true);
    }


    public async Task StoreDeleteListAsync(string list, bool isMainList)
    {
        _helper.DeleteList(list, isMainList);
        if (isMainList) 
        {
            _events.Publish(new MainListsChangedEvent(MainLists));
            StoreUpdateFilteredLists();
            await TaskHelpers.SaveAsync(this, true);
        }
        else
        {
           _events.Publish(new ArchiveListsChangedEvent(ArchiveLists)); 
            await TaskHelpers.SaveAsync(this, false);
        }  

        SelectList(null);
    }
    //GroupListChangedEvent
    //TaskGroupChangedEvent
    public async Task StoreDeleteTaskAsync(BaseTask task, bool isMainList)
    {
        _helper.DeleteTask(task, isMainList);

        if (task.IsImportant == true) StoreUpdateImportantList();

        await TaskHelpers.SaveAsync(this, true);

    }


    public async Task StoreMoveToArchiveAsync(string list)
    {
        if (SelectedList?.ListName != list) return;

        _helper.MoveToArchive(SelectedList);
        _events.Publish(new MainListsChangedEvent(MainLists));
        _events.Publish(new ArchiveListsChangedEvent(ArchiveLists));

        _events.Publish(new GroupListIsArchiveStateChangedEvent(SelectedList, SelectedList.IsArchived));
        StoreUpdateFilteredLists();    

        await TaskHelpers.SaveAsync(this, true);
        await TaskHelpers.SaveAsync(this, false);
    }   
    public async Task StoreMoveToListAsync(string list)
    {
        if (SelectedList?.ListName != list) return;

        _helper.MoveToList(SelectedList);
        _events.Publish(new MainListsChangedEvent(MainLists));
        _events.Publish(new ArchiveListsChangedEvent(ArchiveLists));

        _events.Publish(new GroupListIsArchiveStateChangedEvent(SelectedList, SelectedList.IsArchived));
        StoreUpdateFilteredLists();

        await TaskHelpers.SaveAsync(this, true);
        await TaskHelpers.SaveAsync(this, false);
    }


    public async Task StoreEditList(
        string oldListName,
        string newListName,
        ObservableCollection<TaskGroup>? editedGroups
    ){
        var existing = MainLists.MainLists.FirstOrDefault(l => l.ListName == newListName);
        if (existing != null) return ;

        _helper.EditList(oldListName, newListName, editedGroups);
        SelectedListName = newListName;

        if (newListName == GlobalVariables.Quick) StoreUpdateQuickList();
        else StoreUpdateFilteredLists();  

        StoreUpdateImportantList();  
        _events.Publish(new GroupListChangedEvent(SelectedList?.Groups));

        await TaskHelpers.SaveAsync(this, true);
    }  
    public ObservableCollection<TaskGroup> CloneList()
    {
        return _helper.Clone(SelectedList?.Groups);
    }
}