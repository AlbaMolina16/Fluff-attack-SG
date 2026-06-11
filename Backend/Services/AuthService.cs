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
        /// Valida la informacion del login y permite el acceso o no
        /// </summary>
        /// <param name="loginInfoDto"></param>
        /// <returns>(success = bool, message = string, userId = int)</returns>
        public async Task<(bool success, string message, UserLoginDto? user)> Login(LoginDto loginInfoDto)
        {
            var user = await _userRepository.GetUserByUsername(loginInfoDto.Username);

            if (user == null)
                return (false, "Usuario no existe.", null);

            bool match = BCrypt.Net.BCrypt.Verify(loginInfoDto.Password, user.PasswordHash);

            if (!match)
                return (false, "Contraseña incorrecta.", null);

            UserLoginDto userLoginDto = new()
            {
                Id = user.Id,
                Nickname = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                IdDifficulty = user.IdDifficulty
            };

            return (true, "Login correcto.", userLoginDto);
        }

        /// <summary>
        /// Valida si ya existe un usuario con ese nickName. Si no existe encripta la contraseña y lo crea en bbdd
        /// </summary>
        /// <param name="userId">Identificador del usuario en la tabla users</param>
        /// <param name="idDifficulty">Identificador de la dificultad preferida por el usuario</param>
        /// <returns>Si es satisfactorio, de devuelve el id del usuario creado</returns>
        public async Task<(bool success, string message)> UpdatePreferences(UpdatePreferencesDto dto)
        {
            try
            {
                await _userRepository.UpdatePreferences(dto);
                return (true, "Dificultad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar la dificultad del usuario.");
            }
        }

        public async Task<(bool success, string message)> Register(RegisterDto newUserDto)
        {
            var existing = await _userRepository.GetByUsername(newUserDto.Username);
            if (existing != null)
                return (false, "Ya existe el usuario.");

            var user = new User
            {
                Username = newUserDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password),
                FirstName = newUserDto.FirstName ?? string.Empty,
                LastName = newUserDto.LastName ?? string.Empty,
                Age = newUserDto.Age,
                Handedness = newUserDto.Handedness,
                IdDifficulty = 0,
                CreatedDate = DateTime.UtcNow,
                LogTimestamp = DateTime.UtcNow
            };

            await _userRepository.CreateUser(user);

            return (true, "Usuario registrado correctamente.");
        }

    }
}