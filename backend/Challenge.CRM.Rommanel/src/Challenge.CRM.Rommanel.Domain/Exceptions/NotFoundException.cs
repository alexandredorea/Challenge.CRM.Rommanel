namespace Challenge.CRM.Rommanel.Domain.Exceptions;

public class NotFoundException(string code, string message)
    : DomainException(code, message)
{
}