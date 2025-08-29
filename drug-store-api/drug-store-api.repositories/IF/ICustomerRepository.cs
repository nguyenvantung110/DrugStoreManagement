using drug_store_api.entities.Customers;

namespace drug_store_api.repositories.IF
{
    public interface ICustomerRepository
    {
        Task<Customer> GetUserByPhoneNumber(string phoneNumber);
        Task CreateCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
    }
}
