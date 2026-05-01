using FluffGameApi.Dtos;
using FluffGameApi.Repositories;

namespace FluffGameApi.Services
{
    public class ScoreService
    {
        private readonly IScoreRepository _scoreRepository;

        public ScoreService(IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
        }

        public async Task<(bool success, string message, List<RecentScoreResponseDto> scores)> GetRecentScoresForUser(int userId, int limit)
        {
            try
            {
                var scores = await _scoreRepository.GetRecentScoresByUserId(userId, limit);
                return (true, "Scores retrieved successfully", scores);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving scores: {ex.Message}", null);
            }
        }
    }
}