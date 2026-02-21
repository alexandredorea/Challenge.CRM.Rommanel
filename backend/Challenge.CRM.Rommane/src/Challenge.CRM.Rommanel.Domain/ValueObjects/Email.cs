using Challenge.CRM.Rommanel.Domain.Abstractions;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using System.Net.Mail;

namespace Challenge.CRM.Rommanel.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Address { get; }

    private Email(string address) => Address = address;

    /// <summary>
    /// Cria um e-mail normalizado (lowercase) a partir do valor informado.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="DomainException">Lançada quando o e-mail é inválido.</exception>
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(nameof(Email), "Campo obrigatório.");

        var normalized = value.Trim().ToLowerInvariant();

        if (!IsValidEmail(normalized))
            throw new DomainException(nameof(Email), "O e-mail informado está em formato inválido.");

        return new Email(normalized);
    }

    private static bool IsValidEmail(string value)
    {
        if (value.Length > 254)
            return false;

        try
        {
            var email = new MailAddress(value);
            return email.Address == value;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Address;
    }

    public override string ToString() => Address;
}