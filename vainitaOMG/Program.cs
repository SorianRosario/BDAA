var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseMiddleware<Klkmdw>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/companies", () => Results.Ok(DataStore.Companies));

app.MapGet("/companies/{id}", (int id) =>
{
    var company = DataStore.Companies.FirstOrDefault(c => c.Id == id);
    return company != null ? Results.Ok(company) : Results.NotFound();
});

app.MapPost("/companies", (Company newCompany) =>
{
    newCompany.Id = DataStore.GetNextCompanyId();
    DataStore.Companies.Add(newCompany);
    return Results.Created($"/companies/{newCompany.Id}", newCompany);
});

app.MapPut("/companies/{id}", (int id, Company updatedCompany) =>
{
    var company = DataStore.Companies.FirstOrDefault(c => c.Id == id);
    if (company == null) return Results.NotFound();

    company.Name = updatedCompany.Name;
    return Results.Ok(company);
});

app.MapDelete("/companies/{id}", (int id) =>
{
    var company = DataStore.Companies.FirstOrDefault(c => c.Id == id);
    if (company == null) return Results.NotFound();

    if (company.Employees.Count > 0)
    {
        return Results.BadRequest("No se puede eliminar la compañía porque tiene empleados asignados.");
    }

    DataStore.Companies.Remove(company);
    return Results.NoContent();
});

app.MapDelete("/companies/{id}/with-employees", (int id) =>
{
    var company = DataStore.Companies.FirstOrDefault(c => c.Id == id);
    if (company == null) return Results.NotFound();

    var employees = DataStore.Employees.Where(e => e.CompanyId == id).ToList();
    foreach (var employee in employees)
    {
        DataStore.Employees.Remove(employee);
    }

    DataStore.Companies.Remove(company);
    return Results.NoContent();
});

// CRUD para Empleados
app.MapGet("/employees", () => Results.Ok(DataStore.Employees));

app.MapGet("/employees/{id}", (int id) =>
{
    var employee = DataStore.Employees.FirstOrDefault(e => e.Id == id);
    return employee != null ? Results.Ok(employee) : Results.NotFound();
});

app.MapPost("/employees", (Employee newEmployee) =>
{
    var company = DataStore.Companies.FirstOrDefault(c => c.Id == newEmployee.CompanyId);
    if (company == null) return Results.BadRequest("La compañía no existe.");

    newEmployee.Id = DataStore.GetNextEmployeeId();
    company.Employees.Add(newEmployee);
    DataStore.Employees.Add(newEmployee);

    return Results.Created($"/employees/{newEmployee.Id}", newEmployee);
});

app.MapPut("/employees/{id}", (int id, Employee updatedEmployee) =>
{
    var employee = DataStore.Employees.FirstOrDefault(e => e.Id == id);
    if (employee == null) return Results.NotFound();

    employee.Name = updatedEmployee.Name;
    employee.CompanyId = updatedEmployee.CompanyId;
    return Results.Ok(employee);
});

app.MapDelete("/employees/{id}", (int id) =>
{
    var employee = DataStore.Employees.FirstOrDefault(e => e.Id == id);
    if (employee == null) return Results.NotFound();

    var company = DataStore.Companies.FirstOrDefault(c => c.Id == employee.CompanyId);
    if (company != null)
    {
        company.Employees.Remove(employee);
    }

    DataStore.Employees.Remove(employee);
    return Results.NoContent();
});

app.Run();
public static class DataStore
{
    public static List<Company> Companies { get; set; } = new List<Company>();
    public static List<Employee> Employees { get; set; } = new List<Employee>();
    private static int companyId = 1;
    private static int employeeId = 1;

    public static int GetNextCompanyId()
    {
        return companyId++;
    }

    public static int GetNextEmployeeId()
    {
        return employeeId++;
    }
}

public class Company
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Employee> Employees { get; set; } = new List<Employee>();
}

public class Employee
{
    public int Id { get; set; }
    public required string  Name { get; set; }
    public int CompanyId { get; set; }
}
