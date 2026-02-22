namespace Challenge.CRM.Rommanel.Infrastructure.ExternalServices.ViaCep;

public interface IViaCepService
{
    Task<ViaCepResponse?> GetAddressByPostalCodeAsync(
        string postalCode,
        CancellationToken cancellationToken = default);
}