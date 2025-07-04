using drug_store_api.dtos.Users;

namespace drug_store_api.services.IF
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto> GetUsersByIdAsync(Guid userId);
        Task CreateUser(UserDto userDto);
        Task UpdateUser(UserUpdateDto userDto);
        Task DeleteUser(Guid userId);
    }
}
