using Service.DTOs;
using Service.Exceptions;
using Service.Services.Interfaces;
using Spectre.Console;

namespace CompanyApplication.Controller
{
    public class EmployeeController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        private void PrintError(string message)
        {
            var panel = new Panel($"[red]XƏTA: {message}[/]").BorderColor(Color.Red);
            AnsiConsole.Write(panel);
        }

        private void PrintSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[green]✔ {message}[/]");
        }

        private async Task ExecuteSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (EmployeeNotFoundException ex)   { PrintError(ex.Message); }
            catch (DepartmentNotFoundException ex) { PrintError(ex.Message); }
            catch (ValidationException ex)         { PrintError(ex.Message); }
            catch (Exception ex)                   { PrintError($"Gözlənilməz xəta: {ex.Message}"); }
        }

        private void PrintEmployees(IEnumerable<EmployeeDto> employees, string emptyMessage = "[yellow]Siyahı boşdur.[/]")
        {
            if (!employees.Any())
            {
                AnsiConsole.MarkupLine(emptyMessage);
                return;
            }

            var table = new Table().BorderColor(Color.Cyan).Border(TableBorder.Rounded).Expand();
            table.AddColumn("[cyan]ID[/]");
            table.AddColumn("[cyan]Ad Soyad[/]");
            table.AddColumn("[cyan]Yaş[/]");
            table.AddColumn("[cyan]Ünvan[/]");
            table.AddColumn("[cyan]Department[/]");

            foreach (var emp in employees)
            {
                table.AddRow(emp.Id.ToString(), $"{emp.Name} {emp.Surname}", emp.Age.ToString(), emp.Address, emp.Department?.Name ?? "Yoxdur");
            }

            AnsiConsole.Write(table);
        }

        public async Task CreateAsync(EmployeeCreateDto dto) =>
            await ExecuteSafeAsync(async () =>
            {
                await AnsiConsole.Status().StartAsync("Yaradılır...", async ctx =>
                    await _employeeService.Create(dto));
                PrintSuccess("Employee uğurla yaradıldı.");
            });

        public async Task UpdateAsync(EmployeeUpdateDto dto) =>
            await ExecuteSafeAsync(async () =>
            {
                await AnsiConsole.Status().StartAsync("Yenilənir...", async ctx =>
                    await _employeeService.Update(dto));
                PrintSuccess("Employee uğurla yeniləndi.");
            });

        public async Task DeleteAsync(int id) =>
            await ExecuteSafeAsync(async () =>
            {
                await AnsiConsole.Status().StartAsync("Silinir...", async ctx =>
                    await _employeeService.Remove(id));
                PrintSuccess("Employee uğurla silindi.");
            });

        public async Task PrintByIdAsync(int id) =>
            await ExecuteSafeAsync(async () =>
            {
                var emp = await _employeeService.GetById(id);
                PrintEmployees(new[] { emp! });
            });

        public async Task PrintByAgeAsync(int age) =>
            await ExecuteSafeAsync(async () =>
            {
                var employees = await _employeeService.GetByAgeAsync(age);
                PrintEmployees(employees, "[red]Axtarışa uyğun nəticə tapılmadı.[/]");
            });

        public async Task PrintByDepartmentIdAsync(int departmentId) =>
            await ExecuteSafeAsync(async () =>
            {
                var employees = await _employeeService.GetByDepartmentIdAsync(departmentId);
                PrintEmployees(employees, "[red]Axtarışa uyğun nəticə tapılmadı.[/]");
            });

        public async Task PrintAllByDepartmentNameAsync(string departmentName) =>
            await ExecuteSafeAsync(async () =>
            {
                var employees = await _employeeService.GetAllByDepartmentNameAsync(departmentName);
                PrintEmployees(employees, "[red]Axtarışa uyğun nəticə tapılmadı.[/]");
            });

        public async Task<bool> SearchAsync(string text)
        {
            try
            {
                var employees = await _employeeService.SearchAsync(text);
                if (!employees.Any())
                {
                    AnsiConsole.MarkupLine("[red]Axtarışa uyğun nəticə tapılmadı. Zəhmət olmasa yenidən cəhd edin.[/]");
                    return false;
                }
                PrintEmployees(employees);
                return true;
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
                return false;
            }
        }

        public async Task PrintCountAsync() =>
            await ExecuteSafeAsync(async () =>
            {
                var count = await _employeeService.GetCountAsync();
                var panel = new Panel($"Ümumi işçi sayı: [yellow]{count}[/]").BorderColor(Color.Green);
                AnsiConsole.Write(panel);
            });

        public async Task PrintAllAsync() =>
            await ExecuteSafeAsync(async () =>
            {
                var employees = await _employeeService.GetAll();
                PrintEmployees(employees);
            });
    }
}
