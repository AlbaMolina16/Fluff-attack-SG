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

        /// <summary>
        /// Obtiene los niveles de dificultad incluyendo los parámetros de movimiento si tienen alguno relacionado
        /// </summary>
        /// <returns>Lista de dificultades</returns>
        public async Task<List<DifficultyDto>> GetAll()
        {
            string sql = @"
                SELECT d.Id, d.Name, d.EnemySpeed, d.EnemyLifeTime, d.SpawnRate, d.AmountEnemies,
                       mt.Name, dmt.Probability, dmt.MinSpeed, dmt.MaxSpeed
                FROM difficulties d
                LEFT JOIN difficulty_movementType dmt ON d.Id = dmt.IdDifficulty
                LEFT JOIN movement_type mt ON dmt.IdMovementType = mt.Id
                ORDER BY d.Id";

            var difficultyDict = new Dictionary<int, DifficultyDto>();
            // multi-mapping de Dapper para poder relacionar en DifficultyDto los registros de Movement
            await Connection.QueryAsync<DifficultyDto, DifficultyMovementTypeDto, DifficultyDto>(
                sql,
                (difficulty, movement) =>
                {
                    if (!difficultyDict.TryGetValue(difficulty.Id, out var entry))
                    {
                        entry = difficulty;
                        difficultyDict.Add(entry.Id, entry);
                    }
                    if (movement?.Name != null)
                        entry.Movements.Add(movement);
                    return entry;
                },
                splitOn: "Name");

            return difficultyDict.Values.ToList();
        }
    }
}