using Challenge.CRM.Rommanel.Domain.Enumerators;
using Challenge.CRM.Rommanel.Domain.Events;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.Primitives;
using Challenge.CRM.Rommanel.Domain.Primitives.Abstractions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;

namespace Challenge.CRM.Rommanel.Domain.Entities;

public sealed class Customer : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public DateOnly OriginDate { get; private set; }
    public Document Document { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Telephone Telephone { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public string? StateRegistration { get; private set; }
    public Boolean Active { get; private set; }

    /// <summary>
    ///  Construtor privado — EF Core
    /// </summary>
    private Customer()
    { }

    private Customer(
        string name,
        DateOnly originDate,
        Document document,
        Email email,
        Telephone telephone,
        Address address,
        string? stateRegistration)
    {
        Name = name;
        OriginDate = originDate;
        Document = document;
        Email = email;
        Telephone = telephone;
        Address = address;
        StateRegistration = stateRegistration;
        Active = true;
    }

    public static Customer Create(
        string name,
        string documentNumber,
        DateOnly originDate,
        string email,
        string telephone,
        string postalCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string federativeUnit,
        string? stateRegistration = null,
        string userId = "",
        string correlationId = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(nameof(Name), "Campo obrigatório.");

        var documentVo = Document.Create(documentNumber);
        var emailVo = Email.Create(email);
        var telephoneVo = Telephone.Create(telephone);
        var addressVo = Address.Create(postalCode, street, number, neighborhood, city, federativeUnit);

        ValidateBusinessRules(documentVo, originDate, stateRegistration);

        var customer = new Customer(
            name,
            originDate,
            documentVo,
            emailVo,
            telephoneVo,
            addressVo,
            stateRegistration);

        customer.ApplyChange(new CustomerCreated(customer, userId, correlationId));

        return customer;
    }

    public void UpdateEmail(
        string email,
        string userId = "",
        string correlationId = "")
    {
        EnsureActive(nameof(Email));

        var emailVo = Email.Create(email);

        if (Email == emailVo)
            return;

        this.ApplyChange(new CustomerEmailChanged(emailVo, userId, correlationId));
        Email = emailVo;
    }

    public void UpdateAddress(
        string postalCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string federativeUnit,
        string userId = "",
        string correlationId = "")
    {
        EnsureActive(nameof(Address));

        var addressVo = Address.Create(
            postalCode,
            street,
            number,
            neighborhood,
            city,
            federativeUnit);

        if (Address == addressVo)
            return;

        this.ApplyChange(new CustomerAddressChanged(addressVo, userId, correlationId));
        Address = addressVo;
    }

    public void UpdateTelephone(
        string telephone,
        string userId = "",
        string correlationId = "")
    {
        EnsureActive(nameof(Telephone));

        var telephoneVo = Telephone.Create(telephone);

        if (Telephone == telephoneVo)
            return;

        this.ApplyChange(new CustomerTelephoneChanged(telephoneVo, userId, correlationId));
        Telephone = telephoneVo;
    }

    public void Disable(
        string userId = "",
        string correlationId = "")
    {
        EnsureActive(nameof(Active));

        Active = false;
        this.ApplyChange(new CustomerDisabled(userId, correlationId));
    }

    protected override void Apply(IDomainEvent @event)
    {
        switch (@event)
        {
            case CustomerCreated e:
                Name = e.Customer.Name;
                OriginDate = e.Customer.OriginDate;
                Document = e.Customer.Document;
                Email = e.Customer.Email;
                Telephone = e.Customer.Telephone;
                Address = e.Customer.Address;
                StateRegistration = e.Customer.StateRegistration;
                Active = true;
                break;

            case CustomerEmailChanged e:
                Email = e.Email;
                break;

            case CustomerAddressChanged e:
                Address = e.Address;
                break;

            case CustomerTelephoneChanged e:
                Telephone = e.Telephone;
                break;

            case CustomerDisabled:
                Active = false;
                break;
        }
    }

    private void EnsureActive(string errorCode)
    {
        if (!Active)
            throw new BusinessRuleException(errorCode, "Operação não permitida para clientes inativos.");
    }

    private static void ValidateBusinessRules(
        Document document,
        DateOnly originDate,
        string? stateRegistration)
    {
        if (document.Type == DocumentType.Individual)
            ValidateMinimumAge(originDate);
        else
            ValidateStateRegistration(stateRegistration);
    }

    private static void ValidateMinimumAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age))
            age--;

        if (age < 18)
            throw new BusinessRuleException(nameof(OriginDate), "Pessoa Física deve ter no mínimo 18 anos.");
    }

    private static void ValidateStateRegistration(string? stateRegistration)
    {
        if (string.IsNullOrWhiteSpace(stateRegistration))
            throw new BusinessRuleException(nameof(StateRegistration), "Pessoa Jurídica deve informar a Inscrição Estadual ou marcar como Isento.");
    }
}