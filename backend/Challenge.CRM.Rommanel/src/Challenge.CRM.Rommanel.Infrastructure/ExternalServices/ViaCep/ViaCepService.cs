using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Challenge.CRM.Rommanel.Infrastructure.ExternalServices.ViaCep;

/// <summary>
/// Cliente HTTP para a API ViaCEP.
/// Retry e Circuit Breaker são configurados no HttpClientFactory
/// via Polly no DependencyInjection — sem acoplamento aqui.
/// </summary>
public sealed class ViaCepService(
    HttpClient httpClient,
    ILogger<ViaCepService> logger) : IViaCepService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ViaCepResponse?> GetAddressByPostalCodeAsync(
        string postalCode,
        CancellationToken cancellationToken = default)
    {
        var digits = new string(postalCode.Where(char.IsDigit).ToArray());

        try
        {
            var response = await httpClient
                .GetFromJsonAsync<ViaCepResponse>($"{digits}/json/", JsonOptions, cancellationToken);

            if (response is null || response.Erro == true)
            {
                logger.LogWarning("CEP {PostalCode} não encontrado na API ViaCEP.", digits);
                return null;
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Falha ao consultar ViaCEP para o CEP {PostalCode}.", digits);
            return null;
        }
    }
}