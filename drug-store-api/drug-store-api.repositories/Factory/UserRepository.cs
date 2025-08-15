using drug_store_api.data;
using drug_store_api.entities.Suppliers;
using drug_store_api.entities.Users;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;

namespace drug_store_api.repositories.Factory
{
    public class UserRepository : IUserRepository
    {
        private readonly DrugStoreDbContext _context;
        public UserRepository(DrugStoreDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<User?>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            await _context.Users
            .Where(x => x.UserId == user.UserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.FullName, user.FullName)
                .SetProperty(x => x.Email, user.Email)
                .SetProperty(x => x.PhoneNumber, user.PhoneNumber)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                .SetProperty(x => x.Status, user.Status)
                .SetProperty(x => x.Role, user.Role)
            );
        }

        public async Task UpdateLastLogin(User user)
        {
            await _context.Users
            .Where(x => x.UserId == user.UserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.LastLogin, user.LastLogin)
            );
        }

        public async Task DeleteUser(Guid userId)
        {
            await _context.Users
            .Where(p => p.UserId == userId)
            .ExecuteDeleteAsync();
        }
    }
}
