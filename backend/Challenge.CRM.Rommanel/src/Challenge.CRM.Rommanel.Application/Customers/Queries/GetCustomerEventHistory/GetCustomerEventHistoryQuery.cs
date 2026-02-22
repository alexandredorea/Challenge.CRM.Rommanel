using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Queries.GetCustomerEventHistory;

public sealed record GetCustomerEventHistoryQuery(Guid CustomerId) : IRequest<Result<IReadOnlyList<CustomerEventDto>>>;