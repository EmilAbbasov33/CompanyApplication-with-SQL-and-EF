using Domain.Entities;

namespace Repository.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(int id);
        T? GetById(int id);
        IEnumerable<T> GetAll();
    }
}
