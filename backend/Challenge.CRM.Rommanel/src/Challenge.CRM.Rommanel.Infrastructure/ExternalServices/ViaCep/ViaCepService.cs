using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.Extensions;
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

    public async Task<Result<ViaCepResponse>> GetAddressByPostalCodeAsync(
        string postalCode,
        CancellationToken cancellationToken = default)
    {
        var digits = postalCode.OnlyDigits();

        try
        {
            var response = await httpClient
                .GetFromJsonAsync<ViaCepResponse>($"{digits}/json/", JsonOptions, cancellationToken);

            if (response is null || response.Erro == "true")
            {
                logger.LogWarning("CEP {PostalCode} não encontrado na API ViaCEP.", digits);
                throw new NotFoundException("PostalCode.NotFound", $"CEP '{postalCode}' não encontrado na API ViaCEP.");
            }

            return Result<ViaCepResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Falha ao consultar ViaCEP para o CEP {PostalCode}.", digits);
            //return Result<ViaCepResponse?>.InternalError(ex.Message, "INTERNAL_SERVER_ERROR", $"Falha ao consultar ViaCEP para o CEP {postalCode}.");
            //return null;
            throw;
        }
    }
}