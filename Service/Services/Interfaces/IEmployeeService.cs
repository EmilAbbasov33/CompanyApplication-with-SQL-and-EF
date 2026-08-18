using Service.DTOs;

namespace Service.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task CreateAsync(EmployeeCreateDto dto);
        Task UpdateAsync(EmployeeUpdateDto dto);
        Task DeleteAsync(int id);
        Task<EmployeeDto> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeDto>> GetByAgeAsync(int age);
        Task<IEnumerable<EmployeeDto>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<EmployeeDto>> GetAllByDepartmentNameAsync(string departmentName);
        Task<IEnumerable<EmployeeDto>> SearchAsync(string text);
        Task<int> GetCountAsync();
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
    }
}
