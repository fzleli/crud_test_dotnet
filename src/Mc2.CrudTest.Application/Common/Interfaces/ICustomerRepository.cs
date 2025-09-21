using Mc2.CrudTest.Domain.Entities;

namespace Mc2.CrudTest.Application.Common.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
    }
}
