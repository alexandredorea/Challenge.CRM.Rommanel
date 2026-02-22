using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Entities;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerHandler(
    IAppDbContext context,
    ICurrentUserService userService,
    ICorrelationIdProvider correlationId,
    IEventStore eventStore) : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        // regras de negócio encapsuladas no domínio
        var customer = Customer.Create(
            name: command.Name,
            documentNumber: command.DocumentNumber,
            originDate: command.BirthOrFoundationDate,
            email: command.Email,
            telephone: command.Telephone,
            postalCode: command.PostalCode,
            street: command.Street,
            number: command.AddressNumber,
            neighborhood: command.Neighborhood,
            city: command.City,
            federativeUnit: command.FederativeUnit,
            stateRegistration: command.StateRegistration,
            userId: userService.UserId,
            correlationId: correlationId.Value);

        // Persiste no read model
        await context.Customers.AddAsync(customer, cancellationToken);

        // Persiste eventos no Event Store — mesma transação
        await eventStore.AppendAsync(customer.Id, customer.GetUncommittedEvents(), cancellationToken);

        // Commit atômico: customers + customer_events
        await context.SaveChangesAsync(cancellationToken);

        customer.MarkEventsAsCommitted();

        return Result<CustomerDto>.Ok(customer.ToDto(), "Cliente cadastrado com sucesso.");
    }
}