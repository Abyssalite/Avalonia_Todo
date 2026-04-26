using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class TaskHelpers
{
    public static string InputOrDefault(string? input, string defaultValue) =>
        string.IsNullOrWhiteSpace(input) ? defaultValue : input;

    public static bool IsQuickList(string? listName) =>
        listName == GlobalVariables.Quick;

    public static void Print(object? data)
    {
        Console.WriteLine(JsonSerializer.Serialize(
            data,
            AppJsonContext.Default.ObservableCollectionGroupList));
    }

    private static string GetAppDataPath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(folder, "AppName");
        Directory.CreateDirectory(appFolder);
        return appFolder;
    }

    public static async Task SaveAsync(Store store, bool isMain = false)
    {
        string json = isMain
            ? JsonSerializer.Serialize(store.MainLists.MainLists, AppJsonContext.Default.ObservableCollectionGroupList)
            : JsonSerializer.Serialize(store.ArchiveLists.ArchivedLists, AppJsonContext.Default.ObservableCollectionGroupList);

        string path = Path.Combine(GetAppDataPath(), isMain ? "Tasks.json" : "Archive.json");

        await File.WriteAllTextAsync(path, json);
        Console.WriteLine("Saved to: " + path);
    }

    public static async Task<ObservableCollection<GroupList>?> LoadAsync(bool isMain = false)
    {
        string path = Path.Combine(GetAppDataPath(), isMain ? "Tasks.json" : "Archive.json");

        try
        {
            Console.WriteLine("Loading...");

            if (!File.Exists(path))
            {
                Console.WriteLine("File not found. Creating new list.");
                return null;
            }

            string json = await File.ReadAllTextAsync(path);
            Console.WriteLine("Loaded from: " + path);

            return JsonSerializer.Deserialize(
                       json,
                       AppJsonContext.Default.ObservableCollectionGroupList)
                   ?? null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
            return null;
        }
    }
}