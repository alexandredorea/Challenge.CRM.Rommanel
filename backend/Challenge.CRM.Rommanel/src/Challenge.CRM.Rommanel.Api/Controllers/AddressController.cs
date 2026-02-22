using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Infrastructure.ExternalServices.ViaCep;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.CRM.Rommanel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AddressController : ControllerBase
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
            CancellationToken ct = default)
        {
            var address = await viaCepService.GetAddressByPostalCodeAsync(postalCode, ct);
            return Ok(address);
        }
    }
}