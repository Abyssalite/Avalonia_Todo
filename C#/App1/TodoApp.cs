using System;
using System.Collections.Generic;
using System.IO;

class TodoApp {
    private List<BaseTask> tasks = new List<BaseTask>();
    private const string filepath = "tasks.txt";
    
    public void Run() {
        Console.WriteLine("\n--- To-Do List ---");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. View Tasks");
        Console.WriteLine("3. Mark Task as Done");
        Console.WriteLine("4. Delete Task");
        Console.WriteLine("5. Save Tasks");
        Console.WriteLine("6. Load Tasks");
        Console.WriteLine("0. Exit");
        Console.Write("Choose an option: ");
        string input = Console.ReadLine() ?? " ";
    
        switch (input) {
            case "1": Addtask(); break;
            case "0": return;
            default: Console.WriteLine("Invalid choice."); break;
        }
    }

    private void Addtask() {
        Console.Write("Enter task name ");
        string name = Console.ReadLine() ?? " ";
        Console.Write("Enter task description ");
        string desc = Console.ReadLine() ?? " ";
        tasks.Add(new BaseTask { Name = name, Description = desc, IsDone = false });
        Console.WriteLine(tasks.Last().Name + " added.");

    }
    
}