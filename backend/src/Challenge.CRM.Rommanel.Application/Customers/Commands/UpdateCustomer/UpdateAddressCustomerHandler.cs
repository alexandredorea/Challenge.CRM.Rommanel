using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateAddressCustomerHandler(
    IAppDbContext context,
    ICurrentUserService userService,
    ICorrelationIdProvider correlationId,
    IEventStore eventStore) : IRequestHandler<UpdateAddressCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(UpdateAddressCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FindAsync([command.CustomerId], cancellationToken)
            ?? throw new NotFoundException("Customer.NotFound", $"Cliente '{command.CustomerId}' não encontrado.");

        customer.UpdateAddress(
            command.PostalCode, command.Street, command.AddressNumber,
            command.Neighborhood, command.City, command.FederativeUnit,
            userService.UserId, correlationId.Value);

        await eventStore.AppendAsync(customer.Id, customer.GetUncommittedEvents(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        customer.MarkEventsAsCommitted();

        return Result<CustomerDto>.Ok(customer.ToDto(), "Endereço atualizado com sucesso.");
    }
}