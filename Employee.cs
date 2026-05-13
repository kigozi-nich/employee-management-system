// Employee class stores information for one employee.
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public string Position { get; set; }

    // Constructor creates a new employee object.
    public Employee(int id, string name, string department, string position)
    {
        Id = id;
        Name = name;
        Department = department;
        Position = position;
    }

    // Converts employee information into readable text.
    public override string ToString()
    {
        return $"ID: {Id} | Name: {Name} | Department: {Department} | Position: {Position}";
    }
}