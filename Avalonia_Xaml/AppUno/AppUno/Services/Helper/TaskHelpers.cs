using System.Collections.ObjectModel;
using System.Text.Json;

public static class TaskHelpers
{
    public static string InputOrDefault(string? input, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }
    
    public static void AddTaskToCategory(BaseTask task, Store store)
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
        _ = SaveAsync(store.GroupedList);
    }
    
    public static void AddList(string listName, Store store)
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
        _ = SaveAsync(store.GroupedList);
    }

    public static void DeleteTask(BaseTask task,  Store store)
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
        _ = SaveAsync(store.GroupedList);
    }

    public static void DeleteList(string listName,  Store store)
    {
        var list = store.GroupedList.FirstOrDefault(l => l.List == listName);
        if (list == null) return;

        store.GroupedList.Remove(list);
        _ = SaveAsync(store.GroupedList);
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

    public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

   public static async Task SaveAsync(ObservableCollection<GroupList> groupLists, string fileName = "tasks.json")
    {
        string json = JsonSerializer.Serialize(groupLists, JsonOptions);

        StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
            fileName,
            CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteTextAsync(file, json);

        Console.WriteLine("Saved to: " + ApplicationData.Current.LocalFolder.Path);
    }

    public static async Task<ObservableCollection<GroupList>> LoadAsync(string fileName = "tasks.json")
    {
        try
        {
            Console.WriteLine("Loading...");
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            string json = await FileIO.ReadTextAsync(file);
            Console.WriteLine("Loaded from: " + file.Path);

            return JsonSerializer.Deserialize<ObservableCollection<GroupList>>(json, JsonOptions)
                   ?? new ObservableCollection<GroupList>();
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found. Creating new list.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
        }

        // Default fallback list
        return new ObservableCollection<GroupList>
        {
            new GroupList
            {
                List = "Quick",
                Groups = new ObservableCollection<TaskGroup>()
            }
        };
    }
}