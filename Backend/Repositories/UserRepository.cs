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
        /// Obtiene todos los usuarios registrados en la base de datos. Devuelve una lista de objetos User con la informacion de cada usuario.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAll()
        {
            var users = await Connection.QueryAsync<User>("SELECT * FROM users");

            return users.ToList();
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
        /// Obtiene un usuario por su nickname junto con sus preferencias y la dificultad asociada
        /// </summary>
        public async Task<(User? user, UserPreferencesDto? preferences)> GetByUsernameWithPreferences(string username)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE Username = @Username",
                new { Username = username });

            if (user == null) return (null, null);

            var preferences = await connection.QueryFirstOrDefaultAsync<UserPreferencesDto>(
                @"SELECT up.Id, up.IdDifficulty, d.Name AS DifficultyName
                  FROM user_preferences up
                  INNER JOIN difficulties d ON d.Id = up.IdDifficulty
                  WHERE up.IdUser = @UserId",
                new { UserId = user.Id });

            return (user, preferences);
        }

        /// <summary>
        /// Crea un usuario nuevo y le añade por defecto las preferencias del juego con una dificultad facil
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<int> CreateUser(User user)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));
            await connection.OpenAsync();

            // Se inicia una transaccion para escribir registros en las dos tablas. En caso de fallar en la ejecucion de alguno de ellos
            // hara Rollback y no se informara ningun dato en la bbdd
            using var transaction = connection.BeginTransaction();

            try
            {
                string insertUser = @"
                    INSERT INTO users (Username, FirstName, LastName, Age, Handedness, PasswordHash, CreatedDate, LogTimestamp)
                    VALUES (@Username, @FirstName, @LastName, @Age, @Handedness, @PasswordHash, @CreatedDate, @LogTimestamp);
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
                    user.LogTimestamp
                }, transaction);

                int easyDifficultyId = await connection.ExecuteScalarAsync<int>(
                    "SELECT Id FROM difficulties WHERE Name = 'easy'",
                    transaction: transaction);

                string insertPreferences = @"
                    INSERT INTO user_preferences (IdUser, IdDifficulty, LogTimestamp)
                    VALUES (@IdUser, @IdDifficulty, @LogTimestamp)";

                await connection.ExecuteAsync(insertPreferences, new
                {
                    IdUser = newUserId,
                    IdDifficulty = easyDifficultyId,
                    LogTimestamp = DateTime.UtcNow
                }, transaction);

                transaction.Commit();
                return newUserId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdatePreferences(int preferencesId, int idDifficulty)
        {
            string sql = "UPDATE user_preferences SET IdDifficulty = @IdDifficulty WHERE Id = @Id";
            await Connection.ExecuteAsync(sql, new { IdDifficulty = idDifficulty, Id = preferencesId });
        }

    }
}