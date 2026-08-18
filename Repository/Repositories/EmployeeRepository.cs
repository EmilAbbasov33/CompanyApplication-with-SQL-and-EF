using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {

        public async Task<IEnumerable<Employee>> GetByAgeAsync(int age)
        {
            return await _dbSet.Where(e => e.Age == age).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _dbSet.Where(e => e.DepartmentId == departmentId).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return await GetAllAsync();

            return await _dbSet
                .Where(e => (e.Name != null && e.Name.Contains(text)) ||
                            (e.Surname != null && e.Surname.Contains(text)))
                .ToListAsync();
        }
    }
}
