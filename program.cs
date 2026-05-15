using EmployeeManagement.Services;

EmployeeManager manager = new EmployeeManager();

bool running = true;

while (running)
{
    Console.WriteLine("\n===== Employee Management System =====");
    Console.WriteLine("1. Add Employee");
    Console.WriteLine("2. View Employees");
    Console.WriteLine("3. Search Employee by ID");
    Console.WriteLine("4. Search Employees by Department");
    Console.WriteLine("5. Edit Employee");
    Console.WriteLine("6. Delete Employee");
    Console.WriteLine("7. Save Employees");
    Console.WriteLine("8. Load Employees");
    Console.WriteLine("9. Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            manager.AddEmployee();
            break;
        case "2":
            manager.ViewEmployees();
            break;
        case "3":
            manager.SearchById();
            break;
        case "4":
            manager.SearchByDepartment();
            break;
        case "5":
            manager.EditEmployee();
            break;
        case "6":
            manager.DeleteEmployee();
            break;
        case "7":
            manager.SaveEmployees();
            break;
        case "8":
            manager.LoadEmployees();
            break;
        case "9":
            running = false;
            Console.WriteLine("Goodbye!");
            break;
        default:
            Console.WriteLine("Invalid option. Choose 1 to 9.");
            break;
    }
}
