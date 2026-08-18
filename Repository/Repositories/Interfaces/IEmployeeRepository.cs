using Domain.Entities;

namespace Repository.Repositories.Interfaces
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetByAgeAsync(int age);
        Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<Employee>> SearchAsync(string text);
    }
}
