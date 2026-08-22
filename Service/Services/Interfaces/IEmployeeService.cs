using Domain.Entities;
using Service.DTOs.EmployeeDTO;

namespace Service.Services.Interfaces
{
    public interface IEmployeeService
        : ICrudService<Employee, EmployeeDto, EmployeeCreateDto, EmployeeUpdateDto>
    {
        Task<IEnumerable<EmployeeDto>> GetByAgeAsync(int age);
        Task<IEnumerable<EmployeeDto>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<EmployeeDto>> GetAllByDepartmentNameAsync(string departmentName);
        Task<IEnumerable<EmployeeDto>> SearchAsync(string text);
        Task<int> GetCountAsync();
    }
}
