using Domain.Entities;
using Service.DTOs.DepartmentDTO;
using Service.Enums;

namespace Service.Services.Interfaces
{
    public interface IDepartmentService : ICrudService<Department, DepartmentDto, DepartmentCreateDto, DepartmentUpdateDto>
    {
        Task<IEnumerable<DepartmentDto>> SearchAsync(string text);
        Task<IEnumerable<DepartmentDto>> SortByCapacityAsync(SortDirection direction);
    }
}
