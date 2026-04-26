using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_EventHub;
using App1.Events;
using App1.ViewModels;
using System.Threading.Tasks;

public class Store : ModelBase
{
    private StoreHelpers _helper;

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
    public void StoreUpdateQuickList()
    {
        QuickList = MainLists.MainLists.FirstOrDefault(g => g.ListName == GlobalVariables.Quick);
        _events.Publish(new QuickListsChangedEvent(QuickList));
    }

    public void SelectList(GroupList? list)
    {
        SelectedList = list;
        SelectedListName = list?.ListName;
        TopbarText = SelectedListName;
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

        StoreUpdateImportantList();

    }
    public void SetTaskDone(bool? value)
    {
        if (SelectedTask == null) return;
        SelectedTask.IsDone = value;
    }
    public void OnChangeListName(bool value, string name)
    {
        _events.Publish(new ChangeListNameEvent(value, name));
    }
    public void EditTopBarText(string? text)
    {
        if (text == null) return;
        TopbarText = text;
        _events.Publish(new TopbarTextChangedEvent(text));
    }
    
    public void EditSelectListName(string? text)
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
    public async Task StoreAddTaskToCategoryAsync(BaseTask task, string listname)
    {
        _helper.AddTaskToCategory(task);

        if (listname == GlobalVariables.Important) StoreUpdateImportantList();
        else if (listname == GlobalVariables.Quick) StoreUpdateQuickList();

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
        _helper.MoveToArchive(list);
        _events.Publish(new MainListsChangedEvent(MainLists));
        _events.Publish(new ArchiveListsChangedEvent(ArchiveLists));

        StoreUpdateFilteredLists();    

        await TaskHelpers.SaveAsync(this, true);
        await TaskHelpers.SaveAsync(this, false);
    }   
    public async Task StoreMoveToListAsync(string list)
    {
        _helper.MoveToList(list);
        _events.Publish(new MainListsChangedEvent(MainLists));
        _events.Publish(new ArchiveListsChangedEvent(ArchiveLists));

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
        EditSelectListName(newListName);

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