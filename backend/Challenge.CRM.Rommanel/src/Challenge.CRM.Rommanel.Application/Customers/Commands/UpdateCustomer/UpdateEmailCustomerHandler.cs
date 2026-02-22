using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateEmailCustomerHandler(
    IAppDbContext context,
    ICurrentUserService userService,
    ICorrelationIdProvider correlationId,
    IEventStore eventStore) : IRequestHandler<UpdateEmailCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(UpdateEmailCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FindAsync([command.CustomerId], cancellationToken)
            ?? throw new DomainException("Customer.NotFound", $"Cliente '{command.CustomerId}' não encontrado.");

        customer.UpdateEmail(command.Email, userService.UserId, correlationId.Value);

        await eventStore.AppendAsync(customer.Id, customer.GetUncommittedEvents(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        customer.MarkEventsAsCommitted();

        return Result<CustomerDto>.Ok(customer.ToDto(), "E-mail atualizado com sucesso.");
    }
}