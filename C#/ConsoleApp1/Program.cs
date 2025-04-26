class Program
{
    static void Main()
    {
        Console.Write("Input your name: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Input your age: ");

        int age = int.TryParse(Console.ReadLine(), out int num)?  num : 0;

        Student s = new Student(name, age);
        s.Greet();       // From Person
        s.ShowInfo();    // From Student

        IHomework h = new Student(name, age);
        Console.Write("Input subject: ");
        string subject = Console.ReadLine() ?? "";
        h.GetResult(h.DoHomework(subject));

    }
}
