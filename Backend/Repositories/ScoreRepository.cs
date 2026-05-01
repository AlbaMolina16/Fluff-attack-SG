using Dapper;
using FluffGameApi.Dtos;
using MySql.Data.MySqlClient;
using System.Data;

namespace FluffGameApi.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly IConfiguration _configuration;

        private IDbConnection Connection => new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));

        public ScoreRepository(IConfiguration config)
        {
            _configuration = config;
        }

        /// <summary>
        /// Obtiene las ultimas puntuaciones recientes de un usuario por su Id
        /// </summary>
        /// <param name="userId">Numero identificador del usuario</param>
        /// <param name="limit">Numero de registros de puntuacion a obtener</param>
        /// <returns></returns>
        public async Task<List<RecentScoreDto>> GetRecentScoresByUserId(int userId, int limit)
        {
            string sql = @"
                SELECT s.TotalPoints, s.IdDifficulty, d.Name AS DifficultyName
                FROM scores s
                JOIN difficulties d ON s.IdDifficulty = d.Id
                WHERE s.IdUser = @UserId
                ORDER BY s.TotalPoints DESC
                LIMIT @Limit";

            var scores = await Connection.QueryAsync<RecentScoreDto>(sql, new { UserId = userId, Limit = limit });
            return scores.ToList();
        }

        /// <summary>
        /// Obtiene de base de datos la última puntuación registrado para el userId indicado
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<LastScoreDto?> GetLastScoreByUserId(int userId)
        {
            string sql = @"
                SELECT s.TotalPoints, s.RedPoints, s.BluePoints, s.GreenPoints, s.YellowPoints, d.Name AS DifficultyName
                FROM scores s
                JOIN difficulties d ON s.IdDifficulty = d.Id
                WHERE s.IdUser = @UserId
                ORDER BY s.LogTimestamp DESC
                LIMIT 1";

            return await Connection.QueryFirstOrDefaultAsync<LastScoreDto>(sql, new { UserId = userId });
        }
    }
}