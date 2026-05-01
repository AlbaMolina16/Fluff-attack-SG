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

        /// <summary>
        /// Obtiene las ultimas puntuaciones de un usuario en funci�n de su Id y de la cantidad de registros que quiere obtener
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="limit">Cantidad de registros de puntuacion</param>
        /// <returns></returns>
        public async Task<(bool success, string message, List<RecentScoreDto> scores)> GetRecentScoresForUser(int userId, int limit)
        {
            try
            {
                var scores = await _scoreRepository.GetRecentScoresByUserId(userId, limit);
                return (true, "Scores retrieved successfully", scores);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving scores: {ex.Message}", []);
            }
        }

        /// <summary>
        /// Obtiene la ultima puntuación registrada para un usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<(bool success, string message, LastScoreDto? score)> GetLastScoreForUser(int userId)
        {
            try
            {
                var score = await _scoreRepository.GetLastScoreByUserId(userId);
                return (true, "Score retrieved successfully", score);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving score: {ex.Message}", null);
            }
        }
    }
}