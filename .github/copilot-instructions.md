# Copilot Code Generation Instructions

## General Guidelines
- Use camelCase for variable and method names.
- Use PascalCase for class and property names.
- Use double quotes for strings.
- Ensure all methods have XML documentation comments.
- Ensure consistent indentation using 2 spaces.
- Use `_` as a prefix for private member variables.
- Separate implementation and design by using interface (`.cs`) and implementation (`.cs`) files where applicable.
- Favor composition over inheritance where possible.

## Specific Instructions
- Use `readonly` for fields that do not change after initialization.
- Use `var` for type inference where appropriate.
- Prefer lambda expressions for anonymous methods.
- Use `StringBuilder` for string concatenation in loops or performance-critical code.
- Ensure all methods handle errors using exceptions.
- Verify that all included namespaces are used.
- Check for proper error handling in all methods.
- Ensure all loops have proper termination conditions.
- Use descriptive names for variables and methods.
- Avoid deeply nested code; refactor into smaller methods if necessary.
- Use `async`/`await` for asynchronous programming.
- Verify that all dependencies are listed in the `.csproj` file.
- Check for any potential performance issues.
- Ensure all abstract base classes have a virtual destructor equivalent (`Dispose` pattern if implementing `IDisposable`).
- Complete all open TODO comments in the code.

## Testing Guidelines
- Use NUnit for all tests. [Reference](https://nunit.org/)
- Ensure all tests are self-contained and do not rely on external state.
- Write tests for all public methods and classes.
- Ensure tests cover both typical and edge cases.
- Use descriptive names for test cases and methods.

### Example Test Snippet
```csharp
using NUnit.Framework;
using MyNamespace;

/**
 * Test case for the Manager class.
 */
[TestFixture]
public class ManagerTests
{
  [Test]
  public void GetDetails_ShouldReturnCorrectDetails()
  {
    var manager = new Manager(1, "Alice", 75000.0, "Engineering");
    Assert.AreEqual("ID: 1, Name: Alice, Salary: 75000, Department: Engineering", manager.GetDetails());
  }

  [Test]
  public void GetDepartment_ShouldReturnCorrectDepartment()
  {
    var manager = new Manager(1, "Alice", 75000.0, "Engineering");
    Assert.AreEqual("Engineering", manager.GetDepartment());
  }
}
```

## Example Code Snippets

```csharp
// Good example of a method to add a new employee with proper naming and error handling
/// <summary>
/// Adds a new employee to the company.
/// </summary>
/// <param name="employeeName">The name of the employee.</param>
/// <param name="employeeId">The ID of the employee.</param>
/// <returns>True if the employee was added successfully, false otherwise.</returns>
public bool AddEmployee(string employeeName, string employeeId)
{
  try
  {
    // Assuming AddEmployeeToDatabase is a method that adds an employee to the database
    var success = AddEmployeeToDatabase(employeeName, employeeId);
    if (!success)
    {
      throw new InvalidOperationException("Failed to add employee to the database");
    }
    return true;
  }
  catch (Exception ex)
  {
    Console.Error.WriteLine($"Failed to add employee: {ex.Message}");
    return false;
  }
}
```

```csharp
// Good example of a method to calculate the department budget with proper naming and error handling
/// <summary>
/// Calculates the budget for a department.
/// </summary>
/// <param name="departmentId">The ID of the department.</param>
/// <returns>The budget of the department.</returns>
public double CalculateDepartmentBudget(string departmentId)
{
  try
  {
    // Assuming GetDepartmentExpenses and GetDepartmentRevenue are methods that retrieve department data
    var expenses = GetDepartmentExpenses(departmentId);
    var revenue = GetDepartmentRevenue(departmentId);
    return revenue - expenses;
  }
  catch (Exception ex)
  {
    Console.Error.WriteLine($"Failed to calculate department budget: {ex.Message}");
    throw;
  }
}
```

```csharp
// Good example of a method to fetch manager details with proper naming and error handling
/// <summary>
/// Fetches the details of a manager.
/// </summary>
/// <param name="managerId">The ID of the manager.</param>
/// <returns>The details of the manager.</returns>
public string FetchManagerDetails(string managerId)
{
  try
  {
    var response = Fetch($"/api/managers/{managerId}");
    if (!response.IsSuccessStatusCode)
    {
      throw new HttpRequestException("Network response was not successful");
    }
    var data = response.Content.ReadAsStringAsync().Result;
    return data;
  }
  catch (Exception ex)
  {
    Console.Error.WriteLine($"Failed to fetch manager details: {ex.Message}");
    throw;
  }
}
```

```csharp
// Good example of a method to update an employee's department with proper naming and error handling
/// <summary>
/// Updates the department of an employee.
/// </summary>
/// <param name="employeeId">The ID of the employee.</param>
/// <param name="newDepartment">The new department to be assigned.</param>
/// <returns>True if the department was updated successfully, false otherwise.</returns>
public bool UpdateEmployeeDepartment(string employeeId, string newDepartment)
{
  try
  {
    // Assuming UpdateDepartmentInDatabase is a method that updates the department in the database
    var success = UpdateDepartmentInDatabase(employeeId, newDepartment);
    if (!success)
    {
      throw new InvalidOperationException("Failed to update employee department in the database");
    }
    return true;
  }
  catch (Exception ex)
  {
    Console.Error.WriteLine($"Failed to update employee department: {ex.Message}");
    return false;
  }
}
```

```csharp
// Good example of a method to generate a report with proper string concatenation using StringBuilder
/// <summary>
/// Generates a report for an employee.
/// </summary>
/// <param name="employeeId">The ID of the employee.</param>
/// <param name="employeeName">The name of the employee.</param>
/// <param name="employeeSalary">The salary of the employee.</param>
/// <returns>A string containing the report of the employee.</returns>
public string GenerateEmployeeReport(string employeeId, string employeeName, double employeeSalary)
{
  var sb = new StringBuilder();
  sb.AppendLine("Employee Report");
  sb.AppendLine($"ID: {employeeId}");
  sb.AppendLine($"Name: {employeeName}");
  sb.AppendLine($"Salary: {employeeSalary}");
  return sb.ToString();
}
```

