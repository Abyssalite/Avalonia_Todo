class BaseTask {
    public required string Name {set; get;}
    public required string Description {set; get;}
    public bool IsDone {set; get;}

    public virtual void Display() {
        Console.WriteLine(Name + (IsDone? " [X]\n" : " [ ]\n") + Description);
    }
}