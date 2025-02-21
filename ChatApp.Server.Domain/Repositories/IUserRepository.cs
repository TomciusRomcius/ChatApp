namespace ChatApp.Domain.Repositories
{
    public interface IUserRepository
    {
        public Task CreateAsync(string username, string email, string passwordHash);
        public Task DeleteAsync(Guid userId);
        public Task FindByIdAsync(Guid userId);
    }
}
