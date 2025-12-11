using Application.Common.Exceptions;
using Mc2.CrudTest.Application.Common.Interfaces;
using MediatR;

namespace Mc2.CrudTest.Application.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventStore _eventStore;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IEventStore eventStore)
    {
        _customerRepository = customerRepository;
        _eventStore = eventStore;
    }

    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
            throw new NotFoundException(string.Empty, request.Id);

        customer.Delete();

        await _customerRepository.DeleteAsync(customer);

        foreach (var domainEvent in customer.DomainEvents)
            await _eventStore.SaveEventAsync(domainEvent);

        customer.ClearDomainEvents();

        return Unit.Value;
    }
}
