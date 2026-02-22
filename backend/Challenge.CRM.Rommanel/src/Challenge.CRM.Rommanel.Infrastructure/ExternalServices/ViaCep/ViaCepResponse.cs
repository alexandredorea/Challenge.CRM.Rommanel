using System.Text.Json.Serialization;

namespace Challenge.CRM.Rommanel.Infrastructure.ExternalServices.ViaCep;

public sealed record ViaCepResponse(
    [property: JsonPropertyName("cep")] string Cep,
    [property: JsonPropertyName("logradouro")] string Logradouro,
    [property: JsonPropertyName("bairro")] string Bairro,
    [property: JsonPropertyName("localidade")] string Localidade,
    [property: JsonPropertyName("uf")] string Uf,
    [property: JsonPropertyName("estado")] string Estado,
    [property: JsonPropertyName("ddd")] string Ddd,
    [property: JsonPropertyName("erro")] bool? Erro
);