using Dapper;
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
        /// Obtiene todos los usuarios registrados en la base de datos. Devuelve una lista de objetos User con la información de cada usuario.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAll()
        {
            var users = await Connection.QueryAsync<User>("SELECT * FROM users");

            return users.ToList();
        }

        public async Task<User?> GetByUsername(string username)
        {
            string sql = "SELECT * FROM users WHERE USERNAME = @Username";

            var user = await Connection.QueryFirstOrDefaultAsync<User>(sql, new
            {
                Username = username
            });

            return user;
        }

    }
}
