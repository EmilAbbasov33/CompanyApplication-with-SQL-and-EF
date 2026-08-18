using Domain.Entities;

namespace Repository.Repositories.Interfaces
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<IEnumerable<Department>> SearchAsync(string text);
    }
}
