using AutoMapper;
using drug_store_api.dtos.Users;
using drug_store_api.entities.Users;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;

namespace drug_store_api.services.Factory
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _iUserRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository iUserRepository, IMapper mapper)
        {
            _iUserRepository = iUserRepository ?? throw new ArgumentNullException(nameof(iUserRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var res = await _iUserRepository.GetUsersAsync();
            List<UserDto> userResponseList = _mapper.Map<List<UserDto>>(res);
            return userResponseList;
        }

        public async Task CreateUser(UserDto userDto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);

            User userToCreate = _mapper.Map<User>(userDto);
            userToCreate.PasswordHash = passwordHash;

            await _iUserRepository.CreateUser(userToCreate);
        }

        public async Task DeleteUser(Guid userId)
        {
            await _iUserRepository.DeleteUser(userId);
        }

        public async Task<UserDto> GetUsersByIdAsync(Guid userId)
        {
            var res = await _iUserRepository.GetByIdAsync(userId);
            UserDto userResponse = _mapper.Map<UserDto>(res);
            return userResponse;
        }

        public async Task UpdateUser(UserUpdateDto userDto)
        {
            try
            {
                var userToUpdate = _mapper.Map<User>(userDto);
                await _iUserRepository.UpdateUser(userToUpdate);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
