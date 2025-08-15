using drug_store_api.entities.Users;

namespace drug_store_api.repositories.IF
{
    public interface IUserRepository
    {
        Task<IEnumerable<User?>> GetUsersAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid userId);
        Task CreateUser(User userDto);
        Task UpdateUser(User userDto);
        Task UpdateLastLogin(User userDto);
        Task DeleteUser(Guid userId);
    }
}
