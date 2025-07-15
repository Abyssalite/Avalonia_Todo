using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public static class TaskHelpers
{
    public static string InputOrDefault(string? input, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    public static bool CheckMainList(string listName) => listName == "Quick" || listName == "Important";
    public static ObservableCollection<TaskGroup> Clone(ObservableCollection<TaskGroup> groupedTasks)
    {
        return new ObservableCollection<TaskGroup>(
            groupedTasks.Select(group => new TaskGroup(group)
            {
                Tasks = group.Tasks,
                Category = group.Category
            })
        );
    }

    public static async Task AddTaskToCategory(BaseTask task, Store store)
    {
        var list = store.Lists.FirstOrDefault(l => l.ListName == task.ListName);
        if (list == null) return;

        var group = list.Groups.FirstOrDefault(g => g.Category == task.Category);
        if (group != null)
        {
            group.Tasks.Add(task);
        }
        else
        {
            // Create new category group if it doesn't exist
            list.Groups.Add(new TaskGroup
            {
                Category = task.Category,
                Tasks = new ObservableCollection<BaseTask> { task }
            });
        }
        await SaveAsync(store);
    }

    public static async Task<bool> AddList(string listName, Store store)
    {
        var list = store.Lists.FirstOrDefault(l => l.ListName == listName);
        if (list != null || listName == "")
            return true;

        // Create new list group if it doesn't exist
        store.Lists.Add(new GroupList
        {
            ListName = listName,
            IsArchived = false,
            Groups = new ObservableCollection<TaskGroup>()
        });
        await SaveAsync(store);
        return false;
    }

    public static async Task DeleteTask(BaseTask task, Store store, bool isArchive = false)
    {
        var list = isArchive ?
            store.Archive.ArchivedLists.FirstOrDefault(l => l.ListName == task.ListName) :
            store.Lists.FirstOrDefault(l => l.ListName == task.ListName);
        if (list == null) return;

        var group = list.Groups.FirstOrDefault(g => g.Category == task.Category);
        if (group == null) return;

        group.Tasks.Remove(task);
        if (group.Tasks.Count == 0)
            list.Groups.Remove(group);

        await SaveAsync(store, isArchive);
    }

    public static async Task DeleteList(string listName, Store store, bool isArchive = false)
    {
        var list = isArchive ?
            store.Archive.ArchivedLists.FirstOrDefault(l => l.ListName == listName) :
            store.Lists.FirstOrDefault(l => l.ListName == listName);
    
        if (list == null) return;

        var newList = isArchive ? store.Archive.ArchivedLists : store.Lists;
        newList.Remove(list);
        await SaveAsync(store, isArchive);
    }

    public static async Task MoveToArchive(string archivingList, Store store)
    {
        var list = store.Lists.FirstOrDefault(l => l.ListName == archivingList);
        if (list == null) return;

        store.Lists.Remove(list);
        await SaveAsync(store);
        list.IsArchived = true;
        store.Archive.ArchivedLists.Add(list);
        await SaveAsync(store, true);
    }

    public static async Task MoveToList(string restoreList, Store store)
    {
        var list = store.Archive.ArchivedLists.FirstOrDefault(l => l.ListName == restoreList);
        if (list == null) return;

        store.Archive.ArchivedLists.Remove(list);
        await SaveAsync(store, true);
        list.IsArchived = false;
        store.Lists.Add(list);
        await SaveAsync(store);
    }

    public static async Task EditList(string oldListname, string newListName, ObservableCollection<TaskGroup>? editedList, Store store)
    {      
        var list = store.Lists.FirstOrDefault(l => l.ListName == oldListname);
        if (list == null || editedList == null) return;
        var ischanged = (oldListname == newListName) ? false : true;

        foreach (var group in editedList)
        {
            group.Category = InputOrDefault(group.Category, "Miscelanious");
            foreach (var task in group.Tasks)
            {
                if (group.Category == task.Category) continue;
                else
                {
                    ischanged = true;
                    task.ListName = newListName;
                    task.Category = group.Category;
                }
            }
        }
        if (ischanged)
            {
                store.SelectedList = new GroupList
                {
                    ListName = newListName,
                    IsArchived = false,
                    Groups = new(editedList)
                };
                store.Lists.Remove(list);
                store.Lists.Add(store.SelectedList);
                await SaveAsync(store);
            }
    }

    public static void print(object? data)
    {
        Console.WriteLine(JsonSerializer.Serialize(data, JsonOptions));
    }

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private static string GetAppDataPath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(folder, "AppName");
        Directory.CreateDirectory(appFolder);
        return appFolder;
    }

    public static async Task SaveAsync(Store store, bool isArchive = false)
    {
        string json = isArchive ?
            JsonSerializer.Serialize(store.Archive.ArchivedLists, JsonOptions) :
            JsonSerializer.Serialize(store.Lists, JsonOptions);

        string path = Path.Combine(GetAppDataPath(), isArchive ? "Archive.json" : "Tasks.json");

        await File.WriteAllTextAsync(path, json);
        Console.WriteLine("Saved to: " + path);
    }

    public static async Task<ObservableCollection<GroupList>> LoadAsync(bool isArchive = false)
    {
        string path = Path.Combine(GetAppDataPath(), isArchive ? "Archive.json" : "Tasks.json");

        try
        {
            Console.WriteLine("Loading...");
            if (!File.Exists(path))
            {
                Console.WriteLine("File not found. Creating new list.");
                return GetDefaultList(isArchive);
            }

            string json = await File.ReadAllTextAsync(path);
            Console.WriteLine("Loaded from: " + path);

            return JsonSerializer.Deserialize<ObservableCollection<GroupList>>(json, JsonOptions)
                   ?? GetDefaultList(isArchive);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
            return GetDefaultList(isArchive);
        }
    }

    private static ObservableCollection<GroupList> GetDefaultList(bool isArchive = false) =>
        isArchive ? [] : new()
        {
            new GroupList
            {
                ListName = "Quick",
                IsArchived = false,
                Groups = new ObservableCollection<TaskGroup>()
            },
            new GroupList
            {
                ListName = "Important",
                IsArchived = false,
                Groups = new ObservableCollection<TaskGroup>()
            },
        };
}