using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// EmployeeManager controls all employee actions.
public class EmployeeManager
{
    private List<Employee> employees = new List<Employee>();
    private string filePath = "employees.txt";

    // Adds a new employee to the list.
    public void AddEmployee()
    {
        int id = ReadEmployeeId("Enter employee ID: ");

        if (employees.Any(e => e.Id == id))
        {
            Console.WriteLine("An employee with this ID already exists.");
            return;
        }

        Console.Write("Enter employee name: ");
        string name = ReadText();

        Console.Write("Enter department: ");
        string department = ReadText();

        Console.Write("Enter position: ");
        string position = ReadText();

        employees.Add(new Employee(id, name, department, position));
        Console.WriteLine("Employee added successfully.");
    }

    // Displays all employees currently stored in the program.
    public void ViewEmployees()
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("No employees found.");
            return;
        }

        Console.WriteLine("\nEmployee Records");
        Console.WriteLine("----------------");

        foreach (Employee employee in employees)
        {
            Console.WriteLine(employee);
        }
    }

    // Searches for an employee by ID.
    public void SearchById()
    {
        int id = ReadEmployeeId("Enter employee ID to search: ");
        Employee? employee = employees.FirstOrDefault(e => e.Id == id);

        if (employee == null)
        {
            Console.WriteLine("Employee not found.");
        }
        else
        {
            Console.WriteLine(employee);
        }
    }

    // Searches employees by department.
    public void SearchByDepartment()
    {
        Console.Write("Enter department name: ");
        string department = ReadText();

        List<Employee> results = employees
            .Where(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (results.Count == 0)
        {
            Console.WriteLine("No employees found in that department.");
            return;
        }

        foreach (Employee employee in results)
        {
            Console.WriteLine(employee);
        }
    }

    // Edits an existing employee record.
    public void EditEmployee()
    {
        int id = ReadEmployeeId("Enter employee ID to edit: ");
        Employee? employee = employees.FirstOrDefault(e => e.Id == id);

        if (employee == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        Console.Write("Enter new name: ");
        employee.Name = ReadText();

        Console.Write("Enter new department: ");
        employee.Department = ReadText();

        Console.Write("Enter new position: ");
        employee.Position = ReadText();

        Console.WriteLine("Employee updated successfully.");
    }

    // Deletes an employee record by ID.
    public void DeleteEmployee()
    {
        int id = ReadEmployeeId("Enter employee ID to delete: ");
        Employee? employee = employees.FirstOrDefault(e => e.Id == id);

        if (employee == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        employees.Remove(employee);
        Console.WriteLine("Employee deleted successfully.");
    }

    // Saves employee records to a text file.
    public void SaveEmployees()
    {
        try
        {
            List<string> lines = new List<string>();

            foreach (Employee employee in employees)
            {
                lines.Add($"{employee.Id},{employee.Name},{employee.Department},{employee.Position}");
            }

            File.WriteAllLines(filePath, lines);
            Console.WriteLine("Employee data saved successfully.");
        }
        catch (Exception error)
        {
            Console.WriteLine($"Error saving data: {error.Message}");
        }
    }

    // Loads employee records from a text file.
    public void LoadEmployees()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No saved employee file found.");
                return;
            }

            employees.Clear();
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length == 4 && int.TryParse(parts[0], out int id))
                {
                    employees.Add(new Employee(id, parts[1], parts[2], parts[3]));
                }
            }

            Console.WriteLine("Employee data loaded successfully.");
        }
        catch (Exception error)
        {
            Console.WriteLine($"Error loading data: {error.Message}");
        }
    }

    // Reads and validates employee ID input.
    private int ReadEmployeeId(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int id))
            {
                return id;
            }

            Console.WriteLine("Invalid ID. Please enter a number.");
        }
    }

    // Reads text input and prevents empty values.
    private string ReadText()
    {
        while (true)
        {
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            Console.Write("Input cannot be empty. Enter again: ");
        }
    }
}