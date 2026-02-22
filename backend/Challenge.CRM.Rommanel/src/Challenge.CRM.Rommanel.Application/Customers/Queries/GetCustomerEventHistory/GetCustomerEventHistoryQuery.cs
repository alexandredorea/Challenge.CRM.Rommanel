using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Queries.GetCustomerEventHistory;

public sealed record GetCustomerEventHistoryQuery(Guid CustomerId) : IRequest<Result<IReadOnlyList<CustomerEventDto>>>;

public sealed class GetCustomerEventHistoryHandler(IAppDbContext repository, IEventStore eventStore)
    : IRequestHandler<GetCustomerEventHistoryQuery, Result<IReadOnlyList<CustomerEventDto>>>
{
    public async Task<Result<IReadOnlyList<CustomerEventDto>>> Handle(
        GetCustomerEventHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var exists = await repository.Customers.FindAsync([query.CustomerId], cancellationToken)
            ?? throw new DomainException("Customer.NotFound", $"Cliente '{query.CustomerId}' não encontrado.");

        var events = await eventStore.LoadAsync(query.CustomerId, cancellationToken);

        var dtos = events.Select(item
            => new CustomerEventDto(
                EventId: item.Id,
                EventType: item.GetType().Name,
                Version: item.Version,
                UserId: item.UserId,
                CorrelationId: item.CorrelationId,
                OccurredAt: item.OccurredAt,
                Payload: item)
            ).ToList();

        return Result<IReadOnlyList<CustomerEventDto>>.Ok(dtos);
    }
}