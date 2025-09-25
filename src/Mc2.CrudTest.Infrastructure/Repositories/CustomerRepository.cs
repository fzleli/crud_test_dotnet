using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mc2.CrudTest.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context; 

    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<Customer?> GetByIdAsync(Guid id) => await _context.Customers.FindAsync(id);

    public async Task<IEnumerable<Customer>> GetAllAsync() => await _context.Customers.ToListAsync();

    public async Task<bool> IsUniqueByNameAndBirthAsync(string firstName, string lastName, DateTime dateOfBirth)
    {
        return !await _context.Customers.AnyAsync(c => c.FirstName == firstName && c.LastName == lastName && c.DateOfBirth.Date == dateOfBirth.Date);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Customers.AnyAsync(c => c.Email.Value == email);
    }
}
