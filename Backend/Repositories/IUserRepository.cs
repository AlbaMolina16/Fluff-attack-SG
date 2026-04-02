using FluffGameApi.Entities;

namespace FluffGameApi.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAll();
    }
}
