using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_EventHub;

public class StoreHelpers
{
    private Store _store;
    private IEventHub _events;

    public StoreHelpers(Store store,  IEventHub events)
    {
        _store = store;
        _events = events;
    }
    
    public bool AddList(string listName)
    {
        var existing = _store.MainLists.MainLists.FirstOrDefault(l => l.ListName == listName);
        if (existing != null)
            return true;

        _store.MainLists.MainLists.Add(new GroupList(_events)
        {
            ListName = listName,
            IsArchived = false,
            Groups = new ObservableCollection<TaskGroup>()
        });

        //await TaskHelpers.SaveAsync(this);
        return false;
    }

    public void AddTaskToCategory(BaseTask task)
    {
        var list = _store.MainLists.MainLists.FirstOrDefault(l => l.ListName == task.ListName);
        if (list == null) return;

        var group = list.Groups.FirstOrDefault(g => g.Category == task.Category);
        if (group != null)
        {
            group.Tasks.Add(task);
        }
        else
        {
            list.Groups.Add(new TaskGroup(_events)
            {
                Category = task.Category,
                Tasks = new ObservableCollection<BaseTask> { task }
            });
        }

        //await TaskHelpers.SaveAsync(this);
    }

    public void DeleteList(string listName, bool isMainList = true)
    {
        var list = isMainList
            ? _store.MainLists.MainLists.FirstOrDefault(l => l.ListName == listName)
            : _store.ArchiveLists.ArchivedLists.FirstOrDefault(l => l.ListName == listName);


        if (list == null) return;

        var target = isMainList ? _store.MainLists.MainLists : _store.ArchiveLists.ArchivedLists;
        target.Remove(list);

        if (_store.SelectedList?.ListName == listName)
            _store.SelectList(null);

        //await TaskHelpers.SaveAsync(this, isArchive);
    }

    public void DeleteTask(BaseTask task, bool isMainList = true)
    {
        var list = isMainList
            ? _store.MainLists.MainLists.FirstOrDefault(l => l.ListName == task.ListName)
            : _store.ArchiveLists.ArchivedLists.FirstOrDefault(l => l.ListName == task.ListName);

        if (list == null) return;

        var group = list.Groups.FirstOrDefault(g => g.Category == task.Category);
        if (group == null) return;

        group.Tasks.Remove(task);
        if (group.Tasks.Count == 0)
            list.Groups.Remove(group);

        //await TaskHelpers.SaveAsync(this, isArchive);
    }

    public void MoveToArchive(string listName)
    {
        var list = _store.MainLists.MainLists.FirstOrDefault(l => l.ListName == listName);
        if (list == null) return;

         _store.MainLists.MainLists.Remove(list);
        //await TaskHelpers.SaveAsync(this);

        list.IsArchived = true;
         _store.ArchiveLists.ArchivedLists.Add(list);
        //await TaskHelpers.SaveAsync(this, true);

        if (_store.SelectedList?.ListName == listName)
            _store.SelectList(list);

    }

    public void MoveToList(string listName)
    {
        var list = _store.ArchiveLists.ArchivedLists.FirstOrDefault(l => l.ListName == listName);
        if (list == null) return;

        _store.ArchiveLists.ArchivedLists.Remove(list);
        //await TaskHelpers.SaveAsync(this, true);

        list.IsArchived = false;
        _store.MainLists.MainLists.Add(list);
        //await TaskHelpers.SaveAsync(this);

        if (_store.SelectedList?.ListName == listName)
            _store.SelectList(list);
    }

    public void EditList(
        string oldListName,
        string newListName,
        ObservableCollection<TaskGroup>? editedGroups)
    {
        var list = _store.MainLists.MainLists.FirstOrDefault(l => l.ListName == oldListName);
        if (editedGroups == null || list == null) return;

        bool isChanged = oldListName != newListName;

        foreach (var group in editedGroups)
        {
            group.Category = TaskHelpers.InputOrDefault(group.Category, "Miscelanious");

            foreach (var task in group.Tasks)
            {
                if (group.Category == task.Category && newListName == task.ListName)
                    continue;

                isChanged = true;
                task.ListName = newListName;
                task.Category = group.Category;
            }
        }

        if (!isChanged) return;

        var updatedList = new GroupList(_events)
        {
            ListName = newListName,
            IsArchived = false,
            Groups = new ObservableCollection<TaskGroup>(editedGroups)
        };

        _store.MainLists.MainLists.Remove(list);
        _store.MainLists.MainLists.Add(updatedList);

        _store.SelectList(updatedList);
        //await TaskHelpers.SaveAsync(this);
    }

    public ObservableCollection<TaskGroup> Clone(ObservableCollection<TaskGroup>? groupedTasks)
    {
        return new ObservableCollection<TaskGroup>(
            groupedTasks?.Select(group => new TaskGroup(group, _events)
            {
                Tasks = group.Tasks,
                Category = group.Category
            }) ?? []
        );
    }

    public ObservableCollection<TaskGroup> FilterImportant()
    {
        ObservableCollection<TaskGroup> groups = new();

        foreach (var list in _store.MainLists.MainLists)
        {
            ObservableCollection<BaseTask> tasks = new();

            foreach (var group in list.Groups)
            foreach (var task in group.Tasks)
            {
                if (task.IsImportant == true)
                    tasks.Add(task);
            }

            if (tasks.Count != 0)
            {
                groups.Add(new TaskGroup(_events)
                {
                    Category = list.ListName,
                    Tasks = tasks
                });
            }
        }
        return groups.Count != 0 ? groups : [];
    }
}