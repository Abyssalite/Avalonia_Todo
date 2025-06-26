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
        var list = store.GroupedList.FirstOrDefault(l => l.List == task.List);
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
        await SaveAsync(store.GroupedList);
    }
    
    public static async Task AddList(string listName, Store store)
    {
        var list = store.GroupedList.FirstOrDefault(l => l.List == listName);
        if (list != null || listName == "")
        {
            return;
        }

        // Create new list group if it doesn't exist
        store.GroupedList.Add(new GroupList
        {
            List = listName,
            Groups = new ObservableCollection<TaskGroup>()
        });
        await SaveAsync(store.GroupedList);
    }

    public static async Task DeleteTask(BaseTask task,  Store store)
    {
        var list = store.GroupedList.FirstOrDefault(l => l.List == task.List);
        if (list == null) return;

        var group = list.Groups.FirstOrDefault(g => g.Category == task.Category);
        if (group == null) return;

        group.Tasks.Remove(task);

        if (group.Tasks.Count == 0)
        {
            list.Groups.Remove(group);
        }
        await SaveAsync(store.GroupedList);
    }

    public static async Task DeleteList(string listName,  Store store)
    {
        var list = store.GroupedList.FirstOrDefault(l => l.List == listName);
        if (list == null) return;

        store.GroupedList.Remove(list);
        await SaveAsync(store.GroupedList);
    }

    public static void print(object data)
    {
        Console.WriteLine(JsonSerializer.Serialize(data, JsonOptions));
    }

    public static void HookSaveToTask(Store store, BaseTask task)
    {
        task.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(BaseTask.IsDone))
            {
                await SaveAsync(store.GroupedList);
            }
        };
    }

 private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private static string GetAppDataPath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(folder, "YourAppName");
        Directory.CreateDirectory(appFolder);
        return appFolder;
    }

    public static async Task SaveAsync(ObservableCollection<GroupList> groupLists, string fileName = "tasks.json")
    {
        string json = JsonSerializer.Serialize(groupLists, JsonOptions);
        string path = Path.Combine(GetAppDataPath(), fileName);

        await File.WriteAllTextAsync(path, json);
        Console.WriteLine("Saved to: " + path);
    }

    public static async Task<ObservableCollection<GroupList>> LoadAsync(string fileName = "tasks.json")
    {
        string path = Path.Combine(GetAppDataPath(), fileName);

        try
        {
            Console.WriteLine("Loading...");
            if (!File.Exists(path))
            {
                Console.WriteLine("File not found. Creating new list.");
                return GetDefaultList();
            }

            string json = await File.ReadAllTextAsync(path);
            Console.WriteLine("Loaded from: " + path);

            return JsonSerializer.Deserialize<ObservableCollection<GroupList>>(json, JsonOptions)
                   ?? GetDefaultList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
            return GetDefaultList();
        }
    }

    private static ObservableCollection<GroupList> GetDefaultList() => new()
    {
        new GroupList
        {
            List = "Quick",
            Groups = new ObservableCollection<TaskGroup>()
        }
    };
}