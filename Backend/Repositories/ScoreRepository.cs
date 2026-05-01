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

        public async Task<List<RecentScoreResponseDto>> GetRecentScoresByUserId(int userId, int limit)
        {
            string sql = @"
                SELECT s.TotalPoints, s.IdDifficulty, d.Name AS DifficultyName
                FROM scores s
                JOIN difficulties d ON s.IdDifficulty = d.Id
                WHERE s.IdUser = @UserId
                ORDER BY s.TotalPoints DESC
                LIMIT @Limit";

            var scores = await Connection.QueryAsync<RecentScoreResponseDto>(sql, new { UserId = userId, Limit = limit });
            return scores.ToList();
        }
    }
}