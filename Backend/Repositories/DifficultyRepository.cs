using Dapper;
using FluffGameApi.Dtos;
using MySql.Data.MySqlClient;
using System.Data;

namespace FluffGameApi.Repositories
{
    public class DifficultyRepository : IDifficultyRepository
    {
        private readonly IConfiguration _configuration;

        private IDbConnection Connection => new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));

        public DifficultyRepository(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task<List<DifficultyDto>> GetAll()
        {
            string sql = @"
                SELECT Id, Name, EnemySpeed, EnemyLifeTime, SpawnRate, AmountEnemies
                FROM difficulties
                ORDER BY Id";

            var result = await Connection.QueryAsync<DifficultyDto>(sql);

            return result.ToList();
        }
    }
}
