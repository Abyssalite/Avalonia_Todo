class NotedTask : BaseTask
{
    public required string Note { get; set; }

    public override void Display()
    {
        Console.WriteLine($"\t{Name} {(IsDone ? " [X]\n" : " [ ]\n")} \t- {Description}\n\t- {Note}");
    }

}