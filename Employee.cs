namespace EmployeeManagement.Models;

public class Employee
{
    public int Id { get; }
    public string Name { get; set; }
    public string Department { get; set; }
    public string Position { get; set; }

    public Employee(int id, string name, string department, string position)
    {
        Id = id;
        Name = name.Trim();
        Department = department.Trim();
        Position = position.Trim();
    }

    public override string ToString() =>
        $"[ID: {Id,4}] {Name,-20} | Dept: {Department,-15} | Position: {Position}";
}