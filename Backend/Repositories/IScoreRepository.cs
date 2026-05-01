using FluffGameApi.Dtos;

namespace FluffGameApi.Repositories
{
    public interface IScoreRepository
    {
        Task<List<RecentScoreDto>> GetRecentScoresByUserId(int userId, int limit);
        Task<LastScoreDto?> GetLastScoreByUserId(int userId);
    }
}
