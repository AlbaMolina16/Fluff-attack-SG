using FluffGameApi.Dtos;
using FluffGameApi.Entities;

namespace FluffGameApi.Repositories
{
    public interface IScoreRepository
    {
        Task<List<RecentScoreDto>> GetRecentScoresByUserId(int userId, int limit);
        Task<LastScoreDto?> GetLastScoreByUserId(int userId);
        Task<int> CreateScore(Score score);
    }
}