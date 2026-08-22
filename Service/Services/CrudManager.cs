using AutoMapper;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class CrudManager<TEntity, TDto, TCreateDto, TUpdateDto> : ICrudService<TEntity, TDto, TCreateDto, TUpdateDto> where TEntity : BaseEntity
    {
        protected readonly IBaseRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public CrudManager(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public virtual async Task Create(TCreateDto createDto)
        {
            var entity = Mapper.Map<TEntity>(createDto);
            await Repository.CreateAsync(entity);
        }

        public virtual async Task<IEnumerable<TDto>> GetAll()
        {
            var entities = await Repository.GetAllAsync();
            return Mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<TDto?> GetById(int id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
                return default;
            return Mapper.Map<TDto>(entity);
        }

        public virtual async Task Remove(int id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception($"Entity with ID {id} not found.");
            await Repository.DeleteAsync(id);
        }

        public virtual async Task Update(TUpdateDto updateDto)
        {
            var entity = Mapper.Map<TEntity>(updateDto);
            await Repository.UpdateAsync(entity);
        }
    }
}
