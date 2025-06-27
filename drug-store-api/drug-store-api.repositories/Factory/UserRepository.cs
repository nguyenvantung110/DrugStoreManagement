using drug_store_api.data;
using drug_store_api.entities.User;
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
    }
}
