using System;

class Program
{
    static void Main(string[] args)
    {
        Footballer footballer = new Footballer("Anas", 21, "Al Ahly");
        Swimmer swimmer = new Swimmer("Ahmed", 22, "Freestyle");
        Runner runner = new Runner("Omar", 24, 10);

        Athlete[] athletes = { footballer, swimmer, runner };

        foreach (Athlete athlete in athletes)
        {
            athlete.DisplayInfo();
            Console.WriteLine($"Sport: {athlete.SportType}");
            athlete.PerformActivity();
            Console.WriteLine("-------------------------");
        }

        footballer.ScoreGoal();
        swimmer.ShowSwimmingStyle();
        runner.ShowRunningDistance();
    }
}

abstract class Athlete
{
    public string Name { get; set; }
    public int Age { get; set; }

    public abstract string SportType { get; }

    public Athlete(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public abstract void PerformActivity();

    public void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Age: {Age}");
    }
}

class Footballer : Athlete
{
    public string TeamName { get; set; }

    public Footballer(string name, int age, string teamName)
        : base(name, age)
    {
        TeamName = teamName;
    }

    public override string SportType => "Football";

    public override void PerformActivity()
    {
        Console.WriteLine($"{Name} is playing football.");
    }

    public void ScoreGoal()
    {
        Console.WriteLine($"{Name} scored a goal for {TeamName}!");
    }
}

class Swimmer : Athlete
{
    public string SwimmingStyle { get; set; }

    public Swimmer(string name, int age, string swimmingStyle)
        : base(name, age)
    {
        SwimmingStyle = swimmingStyle;
    }

    public override string SportType => "Swimming";

    public override void PerformActivity()
    {
        Console.WriteLine($"{Name} is swimming.");
    }

    public void ShowSwimmingStyle()
    {
        Console.WriteLine($"{Name} uses {SwimmingStyle} style.");
    }
}

class Runner : Athlete
{
    public double DistanceKm { get; set; }

    public Runner(string name, int age, double distanceKm)
        : base(name, age)
    {
        DistanceKm = distanceKm;
    }

    public override string SportType => "Running";

    public override void PerformActivity()
    {
        Console.WriteLine($"{Name} is running.");
    }

    public void ShowRunningDistance()
    {
        Console.WriteLine($"{Name} runs {DistanceKm} KM.");
    }
}
