class Student : Person, IHomework
{
    public string Grade { get; set; }

    // Constructor that calls the base class constructor
    public Student(string name, int age) : base(name, age)
    {
        Grade = GetGrade(age);
        Console.WriteLine("Student constructor called.");
    }

    public void ShowInfo()
    {
        Console.WriteLine($"{Name} is {Grade}.");
    }

    private string GetGrade(int age) {
        if(age > 5 && age < 18) {
            return "in grade " + (age - 5).ToString();
        }
        else return "not in school";
    }

    public bool DoHomework(string subject) {
        Console.WriteLine($"Doing {subject}");
        return true;
    }

    public void GetResult(bool isDone = false) {
        string result = isDone? "Passed" : "Failed";
        Console.WriteLine(result);
    }

}
