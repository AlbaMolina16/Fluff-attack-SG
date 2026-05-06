using FluffGameApi.Dtos;
using FluffGameApi.Entities;

namespace FluffGameApi.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAll();
        public Task<User?> GetByUsername(string username);
        public Task<(User? user, UserPreferencesDto? preferences)> GetByUsernameWithPreferences(string username);
        public Task<int> CreateUser(User user);
        public Task UpdatePreferences(int preferencesId, int idDifficulty);
    }
}
