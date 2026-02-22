namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Métodos de extensão para consulta e formatação de
/// Unidades Federativas brasileiras.
/// </summary>
public static class FederativeUnitExtensions
{
    private static readonly IReadOnlyDictionary<string, string> UfMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AC", "Acre"              }, { "AL", "Alagoas"             },
            { "AM", "Amazonas"          }, { "AP", "Amapá"               },
            { "BA", "Bahia"             }, { "CE", "Ceará"               },
            { "DF", "Distrito Federal"  }, { "ES", "Espírito Santo"      },
            { "GO", "Goiás"             }, { "MA", "Maranhão"            },
            { "MG", "Minas Gerais"      }, { "MS", "Mato Grosso do Sul"  },
            { "MT", "Mato Grosso"       }, { "PA", "Pará"                },
            { "PB", "Paraíba"           }, { "PE", "Pernambuco"          },
            { "PI", "Piauí"             }, { "PR", "Paraná"              },
            { "RJ", "Rio de Janeiro"    }, { "RN", "Rio Grande do Norte" },
            { "RO", "Rondônia"          }, { "RR", "Roraima"             },
            { "RS", "Rio Grande do Sul" }, { "SC", "Santa Catarina"      },
            { "SE", "Sergipe"           }, { "SP", "São Paulo"           },
            { "TO", "Tocantins"         }
        };

    /// <summary>
    /// Verifica se a string é uma sigla de UF válida (ex: "BA", "ba").
    /// </summary>
    public static bool IsValidFederativeUnit(this string value)
        => !string.IsNullOrWhiteSpace(value) && UfMap.ContainsKey(value);

    /// <summary>
    /// Verifica se a string é um nome de estado válido (ex: "Bahia").
    /// </summary>
    public static bool IsValidStateName(this string value)
        => !string.IsNullOrWhiteSpace(value) && UfMap.Values.Any(v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Retorna o nome do estado a partir de uma sigla. Retorna null se não encontrado.
    /// </summary>
    public static string? ToStateName(this string abbreviation)
        => UfMap.TryGetValue(abbreviation, out var state) ? state : null;

    /// <summary>
    /// Retorna a sigla a partir do nome do estado. Retorna null se não encontrado.
    /// </summary>
    public static string? ToFederativeUnit(this string state)
        => UfMap.FirstOrDefault(p => string.Equals(p.Value, state, StringComparison.OrdinalIgnoreCase)).Key;

    /// <summary>
    /// Normaliza a entrada para sigla maiúscula, independente se veio como sigla ou estado.
    /// Retorna null se inválido.
    /// </summary>
    public static string? NormalizeToFederativeUnit(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.IsValidFederativeUnit())
            return value.ToUpperInvariant();

        if (value.IsValidStateName())
            return value.ToFederativeUnit();

        return null;
    }
}