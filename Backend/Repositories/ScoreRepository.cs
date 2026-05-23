using Dapper;
using FluffGameApi.Dtos;
using FluffGameApi.Entities;
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
                ORDER BY s.LogTimestamp DESC, s.Id DESC
                LIMIT @Limit OFFSET 1";

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
                SELECT s.TotalPoints, s.RedPoints, s.BluePoints, s.GreenPoints, s.YellowPoints, s.MissingPoints, d.Name AS DifficultyName
                FROM scores s
                JOIN difficulties d ON s.IdDifficulty = d.Id
                WHERE s.IdUser = @UserId
                ORDER BY s.LogTimestamp DESC
                LIMIT 1";

            return await Connection.QueryFirstOrDefaultAsync<LastScoreDto>(sql, new { UserId = userId });
        }

        public async Task<int> CreateScore(Score score)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));
            await connection.OpenAsync();

            // Se inicia una transaccion para escribir registros en las dos tablas. En caso de fallar en la ejecucion de alguno de ellos
            // hara Rollback y no se informara ningun dato en la bbdd
            //using var transaction = connection.BeginTransaction();

            try
            {
                string insertScore = @"
                    INSERT INTO scores (IdUser, IdDifficulty, RedPoints, BluePoints, GreenPoints, YellowPoints, MissingPoints, TotalPoints)
                    VALUES (@IdUser, @IdDifficulty, @RedPoints, @BluePoints, @GreenPoints, @YellowPoints, @MissingPoints, @TotalPoints);
                    SELECT LAST_INSERT_ID();";

                int newScoreId = await connection.ExecuteScalarAsync<int>(insertScore, score);

                //int easyDifficultyId = await connection.ExecuteScalarAsync<int>(
                //    "SELECT Id FROM difficulties WHERE Name = 'easy'",
                //    transaction: transaction);

                //string insertPreferences = @"
                //    INSERT INTO user_preferences (IdUser, IdDifficulty, LogTimestamp)
                //    VALUES (@IdUser, @IdDifficulty, @LogTimestamp)";

                //await connection.ExecuteAsync(insertPreferences, new
                //{
                //    IdUser = newUserId,
                //    IdDifficulty = easyDifficultyId,
                //    LogTimestamp = DateTime.UtcNow
                //});

                //transaction.Commit();
                return newScoreId;
            }
            catch
            {
                //transaction.Rollback();
                throw;
            }
        }
    }
}