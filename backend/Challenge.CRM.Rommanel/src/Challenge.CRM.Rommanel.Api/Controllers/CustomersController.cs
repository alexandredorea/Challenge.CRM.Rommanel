using Challenge.CRM.Rommanel.Api.DTOs;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.Customers.Commands.CreateCustomer;
using Challenge.CRM.Rommanel.Application.Customers.Commands.DeleteCustomer;
using Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;
using Challenge.CRM.Rommanel.Application.Customers.Queries.GetCustomerById;
using Challenge.CRM.Rommanel.Application.Customers.Queries.GetCustomerEventHistory;
using Challenge.CRM.Rommanel.Application.Customers.Queries.ListCustomers;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Challenge.CRM.Rommanel.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class CustomersController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Lista clientes com paginação e filtro opcional por nome, documento ou e-mail.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<CustomerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListCustomersQuery(search, page, pageSize), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retorna os dados de um cliente pelo Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retorna o histórico de eventos de um cliente (auditoria).
    /// </summary>
    [HttpGet("{id:guid}/events")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<CustomerEventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventHistory(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCustomerEventHistoryQuery(id), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cadastra um novo cliente (PF ou PJ).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<CustomerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new CreateCustomerCommand(
            Name: request.Name,
            DocumentNumber: request.Document,
            BirthOrFoundationDate: request.BirthOrFoundationDate,
            Email: request.Email,
            Telephone: request.Telephone,
            PostalCode: request.PostalCode,
            Street: request.Street,
            AddressNumber: request.AddressNumber,
            Neighborhood: request.Neighborhood,
            City: request.City,
            FederativeUnit: request.FederativeUnit,
            StateRegistration: request.StateRegistration);
        var result = await mediator.Send(query, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result);
    }

    /// <summary>
    /// Atualiza o endereço do cliente.
    /// </summary>
    [HttpPatch("{id:guid}/address")]
    [ProducesResponseType(typeof(Result<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateAddress(
        Guid id,
        [FromBody] UpdateCustomerAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new UpdateAddressCustomerCommand(
            CustomerId: id,
            PostalCode: request.PostalCode,
            Street: request.Street,
            AddressNumber: request.AddressNumber,
            Neighborhood: request.Neighborhood,
            City: request.City,
            FederativeUnit: request.FederativeUnit), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Atualiza e-mail do cliente.
    /// </summary>
    [HttpPatch("{id:guid}/email")]
    [ProducesResponseType(typeof(Result<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateEmail(
        Guid id,
        [FromBody] UpdateCustomerEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new UpdateEmailCustomerCommand(
            CustomerId: id,
            Email: request.Email), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Atualiza e-mail do cliente.
    /// </summary>
    [HttpPatch("{id:guid}/telephone")]
    [ProducesResponseType(typeof(Result<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateTelephone(
        Guid id,
        [FromBody] UpdateCustomerTelephoneRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new UpdateTelephoneCustomerCommand(
            CustomerId: id,
            Telephone: request.Telephone), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Desativa um cliente. Operação irreversível via API.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Disable(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new DisableCustomerCommand(CustomerId: id), cancellationToken);

        return result.Success ? Ok(result) : Conflict(result);
    }
}