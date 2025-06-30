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
        var list = store.Lists.FirstOrDefault(l => l.ListName == task.ListName);
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
        var list = store.Lists.FirstOrDefault(l => l.ListName == listName);
        if (list == null) return;

        store.Lists.Remove(list);
        await SaveAsync(store, isArchive);
    }

    public static async Task MoveToArchive(string archivingList, Store store)
    {
        var list = store.Lists.FirstOrDefault(l => l.ListName == archivingList);
        if (list == null) return;

        list.IsArchived = true;
        store.Archive.ArchivedLists.Add(list);
        await SaveAsync(store, true);
        store.Lists.Remove(list);
        await SaveAsync(store);
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