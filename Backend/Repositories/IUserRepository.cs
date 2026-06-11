using FluffGameApi.Dtos;
using FluffGameApi.Entities;

namespace FluffGameApi.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetByUsername(string username);
        public Task<User?> GetUserByUsername(string username);
        public Task CreateUser(User user);
        public Task UpdatePreferences(UpdatePreferencesDto dto);
    }
}
