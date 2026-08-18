using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {

        public async Task<IEnumerable<Department>> SearchAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return await GetAllAsync();

            return await _dbSet
                .Where(d => d.Name != null && d.Name.Contains(text))
                .ToListAsync();
        }
    }
}
