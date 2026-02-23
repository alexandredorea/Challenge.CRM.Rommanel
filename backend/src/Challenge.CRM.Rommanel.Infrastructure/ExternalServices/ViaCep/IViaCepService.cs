using Challenge.CRM.Rommanel.Application.Common.Models;

namespace Challenge.CRM.Rommanel.Infrastructure.ExternalServices.ViaCep;

public interface IViaCepService
{
    Task<Result<ViaCepResponse>> GetAddressByPostalCodeAsync(
        string postalCode,
        CancellationToken cancellationToken = default);
}