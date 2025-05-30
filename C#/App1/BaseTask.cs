class BaseTask {
    public Guid Id { get; private set; } = Guid.NewGuid();
    public required string Name { set; get; }
    public  string? Description {set; get;}
    public required bool IsDone {set; get;}

    public virtual void Display() {
        Console.WriteLine(Name + (IsDone? " [X]\n" : " [ ]\n") + "- " + Description);
    }
}