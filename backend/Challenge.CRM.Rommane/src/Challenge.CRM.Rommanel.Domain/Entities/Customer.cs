using Challenge.CRM.Rommanel.Domain.Abstractions;
using Challenge.CRM.Rommanel.Domain.Enumerators;
using Challenge.CRM.Rommanel.Domain.Events;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;

namespace Challenge.CRM.Rommanel.Domain.Entities;

public sealed class Customer : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public DateTime OriginDate { get; private set; }
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
        DateTime originDate,
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
    }

    public static Customer Create(
        string name,
        string documentNumber,
        DateTime originDate,
        string email,
        string telephone,
        string postalCode,
        string street,
        string number,
        string bairro,
        string city,
        string state,
        string? stateRegistration)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(nameof(Name), "Campo obrigatório.");

        var documentVo = Document.Create(documentNumber);
        var emailVo = Email.Create(email);
        var telephoneVo = Telephone.Create(telephone);
        var addressVo = Address.Create(postalCode, street, number, bairro, city, state);

        ValidateBusinessRules(documentVo, originDate, stateRegistration);

        var customer = new Customer(
            name,
            originDate,
            documentVo,
            emailVo,
            telephoneVo,
            addressVo,
            stateRegistration);

        customer.ApplyChange(new CustomerCreated(customer, string.Empty, string.Empty));

        return customer;
    }

    public void UpdateEmail(string email)
    {
        var emailVo = Email.Create(email);

        if (Email == emailVo)
            return;

        this.ApplyChange(new CustomerEmailChanged(emailVo, string.Empty, string.Empty));
        Email = emailVo;
    }

    public void UpdateAddress(
        string postalCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string federativeUnit)
    {
        var addressVo = Address.Create(
            postalCode,
            street,
            number,
            neighborhood,
            city,
            federativeUnit);

        if (Address == addressVo)
            return;

        this.ApplyChange(new CustomerAddressChanged(addressVo, string.Empty, string.Empty));
        Address = addressVo;
    }

    public void UpdateTelephone(string telephone)
    {
        var telephoneVo = Telephone.Create(telephone);

        if (Telephone == telephoneVo)
            return;

        this.ApplyChange(new CustomerTelephoneChanged(telephoneVo, string.Empty, string.Empty));
        Telephone = telephoneVo;
    }

    public void Disable()
    {
        if (!Active)
            return;

        Active = false;
        this.ApplyChange(new CustomerDisabled(string.Empty, string.Empty));
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

            case CustomerDisabled e:
                Active = false;
                break;
        }
    }

    private static void ValidateBusinessRules(
        Document document,
        DateTime originDate,
        string? stateRegistration)
    {
        if (document.Type == TypePerson.Individual)
            ValidateMinimumAge(originDate);
        else
            ValidateStateRegistration(stateRegistration);
    }

    private static void ValidateMinimumAge(DateTime birth)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - birth.Year;

        if (birth.Date > today.AddYears(-age))
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