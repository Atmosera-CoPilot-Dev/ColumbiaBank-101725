using System;
using System.Collections.Generic;
using System.Linq;


public class Employee
{
  public string Name { get; set; }
  public string Department { get; set; }
  public double Salary { get; set; }
}

public class SalaryCalculator
{
  public static double CalculateAverageSalary(List<Employee> employees, string department)
  {
    var departmentEmployees = employees.Where(e => e.Department == department).ToList();

    return departmentEmployees.Average(e => e.Salary);
  }
}

public class Program
{
  public static void Main()
  {
    try
    {
      var employees = new List<Employee>
      {
        new Employee { Name = "Alice", Department = "Engineering", Salary = 75000 },
        new Employee { Name = "Bob", Department = "Engineering", Salary = -50000 }, 
        new Employee { Name = "Charlie", Department = "HR", Salary = 60000 },
        new Employee { Name = "Diana", Department = "Engineering", Salary = 80000 },
        new Employee { Name = "Eve", Department = "HR", Salary = 0 } 
      };

      string department = "Engineering";
      double averageSalary = SalaryCalculator.CalculateAverageSalary(employees, department);

      Console.WriteLine($"The average salary in the {department} department is: {averageSalary}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error: {ex.Message}");
    }
  }
}