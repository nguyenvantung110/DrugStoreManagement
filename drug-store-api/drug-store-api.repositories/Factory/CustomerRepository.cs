using drug_store_api.data;
using drug_store_api.entities.Customers;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;

namespace drug_store_api.repositories.Factory
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DrugStoreDbContext _dbContext;
        public CustomerRepository(DrugStoreDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Customer> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber); 
        }

        public async Task CreateCustomer(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _dbContext.Entry(customer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
