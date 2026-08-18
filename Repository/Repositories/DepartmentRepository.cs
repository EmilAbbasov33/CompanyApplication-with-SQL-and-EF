using Domain.Entities;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {

        public IEnumerable<Department> Search(string text)
        {
            if (string.IsNullOrEmpty(text))
                return GetAll();

            return GetAll()
                .Where(d => d.Name != null && d.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
