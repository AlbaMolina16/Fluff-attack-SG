using FluffGameApi.Dtos;
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

        public async Task<(bool success, string message, int idUsuario)> Login(LoginDto dto)
        {
            var user = await _userRepository.GetByUsername(dto.Username);

            if (user == null)
                return (false, "Usuario no existe", 0);


            bool match = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!match)
                return (false, "Contraseña incorrecta", 0);

            return (true, "Login correcto", user.Id);
        }

        public async Task<(bool success, string message, int idUsuario)> Register(RegisterDto dto)
        {
            var existing = await _userRepository.GetByUsername(dto.Username);
            if (existing != null)
                return (false, "El nickname ya está en uso", 0);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName ?? string.Empty,
                LastName = dto.LastName ?? string.Empty,
                BirthDate = dto.BirthDate,
                CreatedDate = DateTime.UtcNow,
                LogTimestamp = DateTime.UtcNow
            };

            int newId = await _userRepository.CreateUser(user);
            return (true, "Usuario registrado correctamente", newId);
        }

    }
}