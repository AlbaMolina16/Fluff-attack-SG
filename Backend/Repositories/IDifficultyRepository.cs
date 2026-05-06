using FluffGameApi.Dtos;

namespace FluffGameApi.Repositories
{
    public interface IDifficultyRepository
    {
        Task<List<DifficultyDto>> GetAll();
    }
}
