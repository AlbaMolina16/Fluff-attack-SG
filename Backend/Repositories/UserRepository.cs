using Dapper;
using FluffGameApi.Dtos;
using FluffGameApi.Entities;
using MySql.Data.MySqlClient;
using System.Data;

namespace FluffGameApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        private IDbConnection Connection => new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));

        public UserRepository(IConfiguration config)
        {
            _configuration = config;
        }

        /// <summary>
        /// Obtiene un usuario por su nickname
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User?> GetByUsername(string username)
        {
            string sql = "SELECT * FROM users WHERE USERNAME = @Username";

            try
            {
                var user = await Connection.QueryFirstOrDefaultAsync<User>(sql, new
                {
                    Username = username
                });

                return user;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario por su nickname
        /// </summary>
        public async Task<User?> GetUserByUsername(string username)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE Username = @Username",
                new { Username = username });

            if (user == null) return null;

            return user;
        }

        /// <summary>
        /// Crea un usuario nuevo y le añade por defecto las preferencias del juego con una dificultad facil
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task CreateUser(User user)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));
            await connection.OpenAsync();

            // Se inicia una transaccion para escribir registros en las dos tablas. En caso de fallar en la ejecucion de alguno de ellos
            // hara Rollback y no se informara ningun dato en la bbdd
            using var transaction = connection.BeginTransaction();

            try
            {
                int easyDifficultyId = await connection.ExecuteScalarAsync<int>(
                    "SELECT Id FROM difficulties WHERE Name = 'easy'",
                    transaction: transaction);

                user.IdDifficulty = easyDifficultyId;

                string insertUser = @"
                    INSERT INTO users (Username, FirstName, LastName, Age, Handedness, PasswordHash, IdDifficulty, CreatedDate, LogTimestamp)
                    VALUES (@Username, @FirstName, @LastName, @Age, @Handedness, @PasswordHash, @IdDifficulty, @CreatedDate, @LogTimestamp);
                    SELECT LAST_INSERT_ID();";

                int newUserId = await connection.ExecuteScalarAsync<int>(insertUser, new
                {
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Age,
                    user.Handedness,
                    user.PasswordHash,
                    user.CreatedDate,
                    user.IdDifficulty,
                    user.LogTimestamp
                }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdatePreferences(UpdatePreferencesDto dto)
        {
            string sql = "UPDATE users SET IdDifficulty = @IdDifficulty WHERE Id = @Id";
            await Connection.ExecuteAsync(sql, new { dto.IdDifficulty, Id = dto.IdUser });
        }

    }
}