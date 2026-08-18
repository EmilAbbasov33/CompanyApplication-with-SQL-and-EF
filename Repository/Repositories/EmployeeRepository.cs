using Domain.Entities;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {

        public IEnumerable<Employee> GetByAge(int age)
        {
            return GetAll().Where(e => e.Age == age);
        }

        public IEnumerable<Employee> GetByDepartmentId(int departmentId)
        {
            return GetAll().Where(e => e.DepartmentId == departmentId);
        }

        public IEnumerable<Employee> Search(string text)
        {
            if (string.IsNullOrEmpty(text))
                return GetAll();

            return GetAll()
                .Where(e => (e.Name != null && e.Name.Contains(text, StringComparison.OrdinalIgnoreCase)) ||
                            (e.Surname != null && e.Surname.Contains(text, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
