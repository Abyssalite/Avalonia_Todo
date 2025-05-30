class BaseTask {
    public Guid Id { get; private set; } = Guid.NewGuid();
    public required string Name { set; get; }
    public  string? Description {set; get;}
    public required bool IsDone {set; get;}
    public required string Category { get; set; }


    public virtual void Display()
    {
        Console.WriteLine($"\t{Name} {(IsDone ? " [X]\n" : " [ ]\n")} \t- {Description}");
    }
}