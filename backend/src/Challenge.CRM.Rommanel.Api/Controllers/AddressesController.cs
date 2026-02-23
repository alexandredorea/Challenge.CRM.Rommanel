using System.Net.Mime;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Infrastructure.ExternalServices.ViaCep;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.CRM.Rommanel.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public sealed class AddressesController : ControllerBase
{
    /// <summary>
    /// Consulta endereço pelo CEP via ViaCEP.
    /// Usado pelo frontend para autocompletar o formulário.
    /// </summary>
    [HttpGet("{postalCode}")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddressByPostalCode(
        string postalCode,
        [FromServices] IViaCepService viaCepService,
        CancellationToken cancellationToken = default)
    {
        var address = await viaCepService.GetAddressByPostalCodeAsync(postalCode, cancellationToken);
        return Ok(address);
    }
}
