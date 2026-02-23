using Challenge.CRM.Rommanel.Domain.Enumerators;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.Extensions;
using Challenge.CRM.Rommanel.Domain.Primitives;

namespace Challenge.CRM.Rommanel.Domain.ValueObjects;

/// <summary>
/// Value Object unificado que representa um documento brasileiro de pessoa física (CPF)
/// ou jurídica (CNPJ). Armazena apenas os dígitos e expõe a formatação sob demanda.
/// </summary>
public sealed class Document : ValueObject
{
    public string Number { get; } = string.Empty;

    public DocumentType Type { get; }

    /// <summary>
    /// Documento formatado com máscara de acordo com o tipo.
    /// CPF:  "529.982.247-25"
    /// CNPJ: "11.222.333/0001-81"
    /// </summary>
    public string Formatted => Type == DocumentType.Individual
        ? Number.ToCpfFormatted()
        : Number.ToCnpjFormatted();

    private Document()
    {
    }

    private Document(string number, DocumentType type)
    {
        Number = number;
        Type = type;
    }

    /// <summary>
    /// Cria um documento a partir de um CPF ou CNPJ com ou sem formatação.
    /// </summary>
    /// <param name="value">CPF (11 dígitos) ou CNPJ (14 dígitos), com ou sem máscara.</param>
    /// <exception cref="DomainException">
    /// Lançada quando o documento não é um CPF ou CNPJ válido.
    /// </exception>
    public static Document Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{nameof(Document)}{nameof(Number)}", "Campo obrigatório.");

        var digits = value.OnlyDigits();

        return digits.Length switch
        {
            11 when digits.IsValidCpf() => new Document(digits, DocumentType.Individual),
            14 when digits.IsValidCnpj() => new Document(digits, DocumentType.LegalEntity),
            11 => throw new DomainException($"{nameof(Document)}{nameof(Number)}", "O CPF informado não é válido."),
            14 => throw new DomainException($"{nameof(Document)}{nameof(Number)}", "O CNPJ informado não é válido."),
            _ => throw new DomainException($"{nameof(Document)}{nameof(Number)}", "O documento deve ser um CPF (11 dígitos) ou CNPJ (14 dígitos).")
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Number;
        //yield return Type;
    }

    public override string ToString() => Formatted;
}
