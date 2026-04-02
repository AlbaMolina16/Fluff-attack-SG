using FluffGameApi.Entities;
using FluffGameApi.Repositories;

namespace FluffGameApi.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtiene todos los usuarios registrados en la base de datos. Devuelve un mensaje ok si los obtuvo correctamente o un ko si no pudo realizar la operación.
        /// </summary>
        /// <returns></returns>
        public async Task<(bool success, string message, List<User> users)> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAll();
                return (true, "Users retrieved successfully", users);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving users: {ex.Message}", null);
            }
        }
    }
}
