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
                return (false, $"Error retrieving users: {ex.Message}", []);
            }
        }

        /// <summary>
        /// Valida la informacion del login y permite el acceso o no
        /// </summary>
        /// <param name="loginInfoDto"></param>
        /// <returns>(success = bool, message = string, userId = int)</returns>
        public async Task<(bool success, string message, UserLoginDto? user)> Login(LoginDto loginInfoDto)
        {
            var user = await _userRepository.GetByUsername(loginInfoDto.Username);

            if (user == null)
                return (false, "Usuario no existe", null);


            bool match = BCrypt.Net.BCrypt.Verify(loginInfoDto.Password, user.PasswordHash);

            if (!match)
                return (false, "Contraseña incorrecta", null);

            UserLoginDto userLoginDto = new()
            {
                Id = user.Id,
                Nickname = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthday = user.BirthDate
            };

            return (true, "Login correcto", userLoginDto);
        }

        /// <summary>
        /// Valida si ya existe un usuario con ese nickName. Si no existe encripta la contraseña y lo crea en bbdd
        /// </summary>
        /// <param name="newUserDto"></param>
        /// <returns>Si es satisfactorio, de devuelve el id del usuario creado</returns>
        public async Task<(bool success, string message, int idUsuario)> Register(RegisterDto newUserDto)
        {
            var existing = await _userRepository.GetByUsername(newUserDto.Username);
            if (existing != null)
                return (false, "El nickname ya está en uso", 0);

            var user = new User
            {
                Username = newUserDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password),
                FirstName = newUserDto.FirstName ?? string.Empty,
                LastName = newUserDto.LastName ?? string.Empty,
                BirthDate = newUserDto.BirthDate,
                CreatedDate = DateTime.UtcNow,
                LogTimestamp = DateTime.UtcNow
            };

            int newId = await _userRepository.CreateUser(user);
            return (true, "Usuario registrado correctamente", newId);
        }

    }
}