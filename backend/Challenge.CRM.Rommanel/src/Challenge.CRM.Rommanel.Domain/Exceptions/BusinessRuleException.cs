namespace Challenge.CRM.Rommanel.Domain.Exceptions;

public class BusinessRuleException(string code, string message)
    : DomainException(code, message)
{
}