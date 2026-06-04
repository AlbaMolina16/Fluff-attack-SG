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
        public async Task<(bool success, string message, List<User> users)> GetAll()
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
            var (user, preferences) = await _userRepository.GetByUsernameWithPreferences(loginInfoDto.Username);

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
                Age = user.Age,
                Preferences = preferences ?? new() { Id = 0, IdDifficulty = 0, DifficultyName = string.Empty }
            };

            return (true, "Login correcto", userLoginDto);
        }

        /// <summary>
        /// Valida si ya existe un usuario con ese nickName. Si no existe encripta la contraseña y lo crea en bbdd
        /// </summary>
        /// <param name="newUserDto"></param>
        /// <returns>Si es satisfactorio, de devuelve el id del usuario creado</returns>
        public async Task<(bool success, string message)> UpdatePreferences(int preferencesId, int idDifficulty)
        {
            try
            {
                await _userRepository.UpdatePreferences(preferencesId, idDifficulty);
                return (true, "Preferencias actualizadas correctamente");
            }
            catch (Exception ex)
            {
                return (false, $"Error actualizando preferencias: {ex.Message}");
            }
        }

        public async Task<(bool success, string message, int idUsuario)> Register(RegisterDto newUserDto)
        {
            var existing = await _userRepository.GetByUsername(newUserDto.Username);
            if (existing != null)
                return (false, "Ya existe el usuario", 0);

            var user = new User
            {
                Username = newUserDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password),
                FirstName = newUserDto.FirstName ?? string.Empty,
                LastName = newUserDto.LastName ?? string.Empty,
                Age = newUserDto.Age,
                Handedness = newUserDto.Handedness,
                CreatedDate = DateTime.UtcNow,
                LogTimestamp = DateTime.UtcNow
            };

            int newId = await _userRepository.CreateUser(user);
            return (true, "Usuario registrado correctamente", newId);
        }

    }
}