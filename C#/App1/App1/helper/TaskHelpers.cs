using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Linq;

public static class TaskHelpers
{
    public static void AddTaskToCategory(BaseTask task, ObservableCollection<GroupList> GroupList)
    {
        var list = GroupList.FirstOrDefault(l => l.List == task.List);
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
        Save(GroupList);
    }

    public static ObservableCollection<GroupList> DeleteTask(BaseTask task, ObservableCollection<GroupList> GroupList)
    {
        var list = GroupList.FirstOrDefault(l => l.List == task.List);
        if (list == null) return GroupList;

        var group = list.Groups.FirstOrDefault(g => g.Category == task.Category);
        if (group == null) return GroupList;

        group.Tasks.Remove(task);

        if (group.Tasks.Count == 0)
        {
            list.Groups.Remove(group);
        }
        Save(GroupList);
        return GroupList;
    }

    public static void HookSaveToTask(Store store, BaseTask task)
    {
        task.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(BaseTask.IsDone))
            {
                Save(store.GroupedList);
            }
        };

    }
    public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    public static void Save(ObservableCollection<GroupList> GroupLists, string fileName = "tasks.json")
    {
        string json = JsonSerializer.Serialize(GroupLists, JsonOptions);
        File.WriteAllText(fileName, json);
        Console.WriteLine("Saving tasks.json to: " + Directory.GetCurrentDirectory());
    }
    
    public static ObservableCollection<GroupList> Load(string filename = "tasks.json")
    {
        try
        {
            string json = File.ReadAllText(filename);
            Console.WriteLine($"Tasks loaded from {filename}.");
            return JsonSerializer.Deserialize<ObservableCollection<GroupList>>(json, JsonOptions) ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
            Console.WriteLine("Creating...");
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
}