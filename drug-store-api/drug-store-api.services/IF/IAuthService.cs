using drug_store_api.entities.Users;

namespace drug_store_api.services.IF
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string username, string password);
        Task<User?> GetUserByIdAsync(Guid userId);
    }
}
