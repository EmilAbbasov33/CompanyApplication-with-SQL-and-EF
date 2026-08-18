using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface IEmployeeService
    {
        void Create(Employee employee);
        void Update(Employee employee);
        void Delete(int id);
        Employee GetById(int id);
        IEnumerable<Employee> GetByAge(int age);
        IEnumerable<Employee> GetByDepartmentId(int departmentId);
        IEnumerable<Employee> GetAllByDepartmentName(string departmentName);
        IEnumerable<Employee> Search(string text);
        int GetCount();
        IEnumerable<Employee> GetAll();
    }
}
