using Domain.Entities;

namespace Repository.Repositories.Interfaces
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        IEnumerable<Employee> GetByAge(int age);
        IEnumerable<Employee> GetByDepartmentId(int departmentId);
        IEnumerable<Employee> Search(string text);
    }
}
