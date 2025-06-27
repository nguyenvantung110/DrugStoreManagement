using drug_store_api.entities.Users;

namespace drug_store_api.repositories.IF
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid userId);
    }
}
