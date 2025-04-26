using System;

class Person
{
    public string Name { get; set; }
    public int Age { get; set; }


    // Constructor
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
        Console.WriteLine("Person constructor called.");
    }

    public void Greet()
    {
        Console.WriteLine($"Hello, my name is {Name}, i'm {Age}.");
    }
}
