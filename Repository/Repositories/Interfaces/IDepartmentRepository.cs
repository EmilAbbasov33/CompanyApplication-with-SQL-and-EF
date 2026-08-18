using Domain.Entities;

namespace Repository.Repositories.Interfaces
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        IEnumerable<Department> Search(string text);
    }
}
