using FluffGameApi.Entities;

namespace FluffGameApi.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAll();
        public Task<User?> GetByUsername(string username);
        public Task<int> CreateUser(User user);
    }
}
