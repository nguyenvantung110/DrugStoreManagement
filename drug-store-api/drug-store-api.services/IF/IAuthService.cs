using drug_store_api.dtos.Auth;
using drug_store_api.entities.Users;

namespace drug_store_api.services.IF
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(string username, string password);
        Task<User?> GetUserByIdAsync(Guid userId);
    }
}
