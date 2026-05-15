using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public class EmployeeManager
{
    // -------------------------------------------------------------------------
    // Fields & Properties
    // -------------------------------------------------------------------------

    private readonly List<Employee> _employees = new();
    private readonly string _filePath;

    public bool HasUnsavedChanges { get; private set; } = false;

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    public EmployeeManager(string filePath = "employees.txt")
    {
        _filePath = filePath;
    }

    // -------------------------------------------------------------------------
    // Public CRUD Operations
    // -------------------------------------------------------------------------

    public void AddEmployee()
    {
        Console.WriteLine("\n-- Add New Employee --");

        int id = PromptForId("Employee ID: ");

        if (_employees.Any(e => e.Id == id))
        {
            PrintWarning("An employee with that ID already exists.");
            return;
        }

        string name       = PromptForText("Full Name:   ");
        string department = PromptForText("Department:  ");
        string position   = PromptForText("Position:    ");

        _employees.Add(new Employee(id, name, department, position));

        HasUnsavedChanges = true;
        PrintSuccess("Employee added successfully.");
    }

    public void ViewEmployees()
    {
        Console.WriteLine("\n-- All Employees --");

        if (_employees.Count == 0)
        {
            PrintWarning("No employee records found.");
            return;
        }

        PrintDivider();
        foreach (Employee emp in _employees)
            Console.WriteLine(emp);
        PrintDivider();
        Console.WriteLine($"  Total: {_employees.Count} employee(s)");
    }

    public void SearchById()
    {
        Console.WriteLine("\n-- Search by ID --");

        int id = PromptForId("Employee ID: ");
        Employee? employee = FindById(id);

        if (employee is null)
            PrintWarning("No employee found with that ID.");
        else
        {
            PrintDivider();
            Console.WriteLine(employee);
            PrintDivider();
        }
    }

    public void SearchByDepartment()
    {
        Console.WriteLine("\n-- Search by Department --");

        string department = PromptForText("Department: ");

        List<Employee> results = _employees
            .Where(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (results.Count == 0)
        {
            PrintWarning($"No employees found in '{department}'.");
            return;
        }

        PrintDivider();
        foreach (Employee emp in results)
            Console.WriteLine(emp);
        PrintDivider();
        Console.WriteLine($"  Found: {results.Count} employee(s) in '{department}'");
    }

    public void EditEmployee()
    {
        Console.WriteLine("\n-- Edit Employee --");

        int id = PromptForId("Employee ID: ");
        Employee? employee = FindById(id);

        if (employee is null)
        {
            PrintWarning("No employee found with that ID.");
            return;
        }

        Console.WriteLine($"\n  Editing: {employee}");
        Console.WriteLine("  (Press Enter to keep current value)\n");

        string? name = PromptForOptionalText($"New Name       [{employee.Name}]: ");
        if (!string.IsNullOrWhiteSpace(name))
            employee.Name = name.Trim();

        string? department = PromptForOptionalText($"New Department [{employee.Department}]: ");
        if (!string.IsNullOrWhiteSpace(department))
            employee.Department = department.Trim();

        string? position = PromptForOptionalText($"New Position   [{employee.Position}]: ");
        if (!string.IsNullOrWhiteSpace(position))
            employee.Position = position.Trim();

        HasUnsavedChanges = true;
        PrintSuccess("Employee updated successfully.");
    }

    public void DeleteEmployee()
    {
        Console.WriteLine("\n-- Delete Employee --");

        int id = PromptForId("Employee ID: ");
        Employee? employee = FindById(id);

        if (employee is null)
        {
            PrintWarning("No employee found with that ID.");
            return;
        }

        Console.WriteLine($"\n  {employee}");
        Console.Write("\n  Confirm deletion (y/n): ");
        string? confirm = Console.ReadLine();

        if (confirm?.Trim().ToLower() == "y")
        {
            _employees.Remove(employee);
            HasUnsavedChanges = true;
            PrintSuccess("Employee deleted successfully.");
        }
        else
        {
            Console.WriteLine("  Deletion cancelled.");
        }
    }

    // -------------------------------------------------------------------------
    // Persistence
    // -------------------------------------------------------------------------

    public void SaveEmployees()
    {
        Console.WriteLine("\n-- Save Employees --");

        try
        {
            IEnumerable<string> lines = _employees
                .Select(e => $"{e.Id}|{Sanitize(e.Name)}|{Sanitize(e.Department)}|{Sanitize(e.Position)}");

            File.WriteAllLines(_filePath, lines);

            HasUnsavedChanges = false;
            PrintSuccess($"Saved {_employees.Count} employee(s) to '{_filePath}'.");
        }
        catch (IOException ex)
        {
            PrintError($"Failed to save: {ex.Message}");
        }
    }

    public void LoadEmployees()
    {
        Console.WriteLine("\n-- Load Employees --");

        if (!File.Exists(_filePath))
        {
            PrintWarning($"File '{_filePath}' not found.");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(_filePath);
            int loaded  = 0;
            int skipped = 0;

            _employees.Clear();

            foreach (string line in lines)
            {
                if (TryParseLine(line, out Employee? emp) && emp is not null)
                {
                    _employees.Add(emp);
                    loaded++;
                }
                else
                {
                    skipped++;
                }
            }

            HasUnsavedChanges = false;
            PrintSuccess($"Loaded {loaded} employee(s) from '{_filePath}'.");

            if (skipped > 0)
                PrintWarning($"{skipped} record(s) were skipped due to corrupt data.");
        }
        catch (IOException ex)
        {
            PrintError($"Failed to load: {ex.Message}");
        }
    }

    // -------------------------------------------------------------------------
    // Private Helpers — Input
    // -------------------------------------------------------------------------

    private static int PromptForId(string label)
    {
        while (true)
        {
            Console.Write($"  {label}");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (int.TryParse(input, out int id) && id > 0)
                return id;

            PrintWarning("Invalid ID. Please enter a positive integer.");
        }
    }

    private static string PromptForText(string label)
    {
        while (true)
        {
            Console.Write($"  {label}");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            PrintWarning("This field cannot be empty. Please try again.");
        }
    }

    private static string? PromptForOptionalText(string label)
    {
        Console.Write($"  {label}");
        return Console.ReadLine();
    }

    // -------------------------------------------------------------------------
    // Private Helpers — Data
    // -------------------------------------------------------------------------

    private Employee? FindById(int id) =>
        _employees.FirstOrDefault(e => e.Id == id);

    private static bool TryParseLine(string line, out Employee? employee)
    {
        employee = null;
        string[] parts = line.Split('|');

        if (parts.Length != 4 || !int.TryParse(parts[0], out int id) || id <= 0)
            return false;

        if (parts.Any(p => string.IsNullOrWhiteSpace(p)))
            return false;

        employee = new Employee(id, parts[1], parts[2], parts[3]);
        return true;
    }

    private static string Sanitize(string value) =>
        value.Replace("|", "").Trim();

    // -------------------------------------------------------------------------
    // Private Helpers — Console Output
    // -------------------------------------------------------------------------

    private static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ {message}");
        Console.ResetColor();
    }

    private static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ⚠ {message}");
        Console.ResetColor();
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✗ {message}");
        Console.ResetColor();
    }

    private static void PrintDivider() =>
        Console.WriteLine("  " + new string('-', 60));
}