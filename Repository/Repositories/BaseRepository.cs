using Domain.Entities;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly List<T> _dbSet;

        public BaseRepository()
        {
            _dbSet = AppDbContext<T>.datas;
        }

        public void Create(T entity)
        {
            entity.Id = _dbSet.Count > 0 ? _dbSet.Max(e => e.Id) + 1 : 1;
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            var existing = _dbSet.FirstOrDefault(e => e.Id == entity.Id);
            if (existing != null)
            {
                var index = _dbSet.IndexOf(existing);
                _dbSet[index] = entity;
            }
        }

        public void Delete(int id)
        {
            var existing = _dbSet.FirstOrDefault(e => e.Id == id);
            if (existing != null)
            {
                _dbSet.Remove(existing);
            }
        }

        public T? GetById(int id)
        {
            return _dbSet.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet;
        }
    }


}
