using FluffGameApi.Dtos;

namespace FluffGameApi.Repositories
{
    public interface IScoreRepository
    {
        Task<List<RecentScoreResponseDto>> GetRecentScoresByUserId(int userId, int limit);
    }
}
