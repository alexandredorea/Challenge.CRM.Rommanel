using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateTelephoneCustomerHandler(
    IAppDbContext context,
    ICurrentUserService userService,
    ICorrelationIdProvider correlationId,
    IEventStore eventStore) : IRequestHandler<UpdateTelephoneCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(UpdateTelephoneCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FindAsync([command.CustomerId], cancellationToken)
            ?? throw new NotFoundException("Customer.NotFound", $"Cliente '{command.CustomerId}' não encontrado.");

        customer.UpdateTelephone(command.Telephone, userService.UserId, correlationId.Value);

        await eventStore.AppendAsync(customer.Id, customer.GetUncommittedEvents(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        customer.MarkEventsAsCommitted();

        return Result<CustomerDto>.Ok(customer.ToDto(), "Telefone atualizado com sucesso.");
    }
}