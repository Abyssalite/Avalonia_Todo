public class BaseTask {
    public required string Name { set; get; }
    public  string? Description {set; get;}
    public required bool IsDone {set; get;}
    public required string Category { get; set; }
}