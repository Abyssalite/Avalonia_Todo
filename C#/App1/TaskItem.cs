class TaskItem : BaseTask
{
    public string? Category { get; set; }

    public override void Display()
    {
        Console.WriteLine(Name + (IsDone? " [X]\n" : " [ ]\n") + "- " + Description);
    }
}