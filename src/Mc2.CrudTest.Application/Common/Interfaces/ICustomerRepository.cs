using Mc2.CrudTest.Domain.Entities;

namespace Mc2.CrudTest.Application.Common.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<bool> IsUniqueByNameAndBirthAsync(string firstName, string lastName, DateTime dateOfBirth);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUniqueByNameAndBirthAsync(string firstName, string lastName, DateTime dateOfBirth, Guid excludeCustomerId);
        Task<bool> IsEmailUniqueAsync(string email, Guid excludeCustomerId);
    }
}
