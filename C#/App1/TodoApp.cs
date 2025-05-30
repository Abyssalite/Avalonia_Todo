using System.Text.Json;

class TodoApp
{
    private List<TaskItem> tasks = new List<TaskItem>();
    private const string filepath = "tasks.txt";

    public void Run()
    {
        tasks = Load();
        string input = "";
        while (input != "0")
        {
            Console.WriteLine("\n--- To-Do List ---");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. View Tasks");
            Console.WriteLine("3. Toggle Task Complete");
            Console.WriteLine("4. Delete Task");
            Console.WriteLine("0. Save And Exit");
            Console.Write("Choose an option: ");
            input = Console.ReadLine() ?? "0";
            switch (input)
            {
                case "1": Addtask(); break;
                case "2": Viewtask(); break;
                case "3": ToggleComplete(); break;
                case "4": DeleteTask(); break;
                case "0": Save(); return;

                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }
    
    private string? PromptOrDefault(string prompt, string? defaultValue)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    private void Addtask()
    {
        string? name = PromptOrDefault("Enter task name: ", null);

        if (name is not null)
        {
            string? desc = PromptOrDefault("Enter task description: ", null);
            string? category = PromptOrDefault("Enter task category: ", "Miscellaneous");

            tasks.Add(new TaskItem { Name = name, Description = desc, IsDone = false, Category = category });
            Console.WriteLine(tasks.Last().Name + " added.");
        }
        else Console.WriteLine("Task name is required!");
    }

    private bool Viewtask()
    {
        if (tasks.Count() != 0)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.Write($"{i + 1}. ");
                tasks[i].Display();
            }
            return true;
        }
        else
        {
            Console.WriteLine("No task added!");
            return false;

        }
    }

    private void ToggleComplete()
    {
        string input = "";
        bool isTask = true;
        while (isTask && input != "0")
        {
            isTask = Viewtask();
            if (isTask)
            {
                Console.WriteLine("0. Exit");
                Console.Write("Choose a task number: ");
                input = Console.ReadLine() ?? "0";
                if (int.TryParse(input, out int index) && index >= 1 && index <= tasks.Count)
                    tasks[index - 1].IsDone = !tasks[index - 1].IsDone;

                else Console.WriteLine("Invalid task number.");
            }
        }
    }

    private void DeleteTask()
    {
        string input = "";
        bool isTask = true;
        while (isTask && input != "0")
        {
            isTask = Viewtask();
            if (isTask)
            {
                Console.WriteLine("0. Exit");
                Console.Write("Choose a task number: ");
                input = Console.ReadLine() ?? "0";
                if (int.TryParse(input, out int index) && index >= 1 && index <= tasks.Count)
                    tasks.RemoveAt(index - 1);

                else Console.WriteLine("Invalid task number.");
            }
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private void Save(string fileName = "tasks.json")
    {
        string json = JsonSerializer.Serialize(tasks, JsonOptions);
        File.WriteAllText(fileName, json);
        Console.WriteLine("Saving tasks.json to: " + Directory.GetCurrentDirectory());
    }

    private List<TaskItem> Load(string filename = "tasks.json")
    {
        try
        {
            string json = File.ReadAllText(filename);
            Console.WriteLine($"Tasks loaded from {filename}.");
            return JsonSerializer.Deserialize<List<TaskItem>>(json, JsonOptions) ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
            Console.WriteLine("Creating...");
            Save();
            return [];
        }
    }
} 