using Domain.Entities;

namespace Service.Services.Interfaces
{
    public interface ICrudService<TEntity, TDto, TCreateDto, TUpdateDto>
        where TEntity : BaseEntity
    {
        Task<IEnumerable<TDto>> GetAll();
        Task<TDto?> GetById(int id);
        Task Create(TCreateDto createDto);
        Task Update(TUpdateDto updateDto);
        Task Remove(int id);
    }
}
