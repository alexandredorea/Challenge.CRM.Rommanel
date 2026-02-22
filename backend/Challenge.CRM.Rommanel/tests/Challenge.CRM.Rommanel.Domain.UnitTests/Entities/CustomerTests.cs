using Challenge.CRM.Rommanel.Domain.Entities;
using Challenge.CRM.Rommanel.Domain.Enumerators;
using Challenge.CRM.Rommanel.Domain.Events;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.Entities;

public sealed class CustomerTests
{
    private static Customer CreateCustomer(
        string? document = null,
        string? stateRegistration = null,
        string? userId = null,
        string? correlationId = null)
        => Customer.Create(
            name: "Alexandre Dórea",
            documentNumber: document ?? "064.638.705-79",
            originDate: new DateOnly(1982, 5, 6),
            email: "alexandre@email.com",
            telephone: "71998877665",
            postalCode: "44073-200",
            street: "Av. Artemia Pires",
            number: "1000",
            neighborhood: "Registro",
            city: "Feira de Santana",
            federativeUnit: "BA",
            stateRegistration: stateRegistration,
            userId: userId ?? Guid.NewGuid().ToString(),
            correlationId: correlationId ?? Guid.NewGuid().ToString());

    private static Customer CreateCustomerIndividual() =>
        Customer.Create(
            name: "Alexandre Dórea",
            documentNumber: "064.638.705-79",
            originDate: new DateOnly(1982, 5, 6),
            email: "joao@email.com",
            telephone: "71999998888",
            postalCode: "44073-200",
            street: "Av. Artemia Pires",
            number: "1000",
            neighborhood: "Registro",
            city: "Feira de Santana",
            federativeUnit: "BA",
            stateRegistration: null,
            userId: "user-01",
            correlationId: "corr-01");

    private static Customer CreateCustomerLegalEntity() =>
        Customer.Create(
            name: "Empresa LTDA",
            documentNumber: "11.222.333/0001-81",
            originDate: new DateOnly(1982, 5, 6),
            email: "pj@empresa.com",
            telephone: "71999998888",
            postalCode: "44073-200",
            street: "Av. Artemia Pires",
            number: "1000",
            neighborhood: "Registro",
            city: "Feira de Santana",
            federativeUnit: "BA",
            stateRegistration: "110.042.490.114",
            userId: "user-01",
            correlationId: "corr-01");

    [Fact]
    public void Create_PessoaFisica_ShouldEmitCustomerCreatedEvent()
    {
        var customer = CreateCustomerIndividual();

        customer.GetUncommittedEvents().Should().HaveCount(1);
        customer.GetUncommittedEvents().First()
            .Should().BeOfType<CustomerCreated>();
    }

    [Fact]
    public void Create_PessoaFisica_ShouldHaveCorrectState()
    {
        var customer = CreateCustomerIndividual();

        customer.Name.Should().Be("Alexandre Dórea");
        customer.Document.Type.Should().Be(DocumentType.Individual);
        customer.Active.Should().BeTrue();
        customer.Version.Should().Be(1);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        var act = () => Customer.Create(
            name: "",
            documentNumber: "064.638.705-79",
            originDate: new DateOnly(1982, 5, 6),
            email: "alexandre@email.com",
            telephone: "71998877665",
            postalCode: "44073-200",
            street: "Av. Artemia Pires",
            number: "1000",
            neighborhood: "Registro",
            city: "Feira de Santana",
            federativeUnit: "BA",
            stateRegistration: null,
            userId: "user-01",
            correlationId: "corr-01");

        act.Should().Throw<DomainException>()
            .And.Message
            .Should().Be("Campo obrigatório.");
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateActiveCustomer()
    {
        var customer = CreateCustomer();

        customer.Id.Should().NotBeEmpty();
        customer.Name.Should().Be("Alexandre Dórea");
        customer.Active.Should().BeTrue();
        customer.Document.Type.Should().Be(DocumentType.Individual);
    }

    [Fact]
    public void Create_ShouldRaiseCustomerCreatedEvent()
    {
        var customer = CreateCustomer();

        customer.GetUncommittedEvents().Should().HaveCount(1);
        customer.GetUncommittedEvents().Single().Should().BeOfType<CustomerCreated>();
        customer.Version.Should().Be(1);
    }

    [Fact]
    public void Create_WithFutureDate_ShouldThrowDomainException()
    {
        var act = () => Customer.Create(
            name: "Alexandre Dórea",
            documentNumber: "529.982.247-25",
            originDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            email: "alexandre@email.com",
            telephone: "71998877665",
            postalCode: "44073-200",
            street: "Av. Artemia Pires",
            number: "1000",
            neighborhood: "Registro",
            city: "Feira de Santana",
            federativeUnit: "BA",
            userId: Guid.NewGuid().ToString(),
            correlationId: Guid.NewGuid().ToString());

        act.Should().Throw<BusinessRuleException>().WithMessage("Pessoa Física deve ter no mínimo 18 anos.");
    }

    [Fact]
    public void Disable_ShouldSetStatusInactive()
    {
        var customer = CreateCustomer();
        customer.Disable(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        customer.Active.Should().BeFalse();
    }

    [Fact]
    public void Disable_InactiveCustomer_ShouldThrowBusinessRuleException()
    {
        var customer = CreateCustomer();
        customer.Disable(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        customer.MarkEventsAsCommitted();

        var act = () => customer.Disable(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        act.Should().Throw<BusinessRuleException>()
            .And.Message
            .Should().Be("Operação não permitida para clientes inativos.");
    }

    [Fact]
    public void UpdateEmail_WithValidData_ShouldRaiseCustomerEmailChanged()
    {
        var customer = CreateCustomer();
        customer.MarkEventsAsCommitted();

        customer.UpdateEmail("novo@email.com", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        customer.Email.Address.Should().Be("novo@email.com");
        customer.GetUncommittedEvents().Should().HaveCount(1);
        customer.GetUncommittedEvents().Single().Should().BeOfType<CustomerEmailChanged>();
    }

    [Fact]
    public void UpdateEmail_WithSameEmail_ShouldNotEmitEvent()
    {
        var customer = CreateCustomer();
        customer.MarkEventsAsCommitted();

        customer.UpdateEmail(customer.Email.Address, "user-01", "corr-02");

        customer.GetUncommittedEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateAddress_WithValidData_ShouldRaiseCustomerAddressChanged()
    {
        var customer = CreateCustomer();
        customer.MarkEventsAsCommitted();

        customer.UpdateAddress(
            "20040020",
            "Av. Rio Branco",
            "156",
            "Centro",
            "Rio de Janeiro",
            "RJ",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        customer.Address.PostalCode.Number.Should().Be("20040020");
        customer.Address.PostalCode.Formatted.Should().Be("20040-020");
        customer.Address.Street.Should().Be("Av. Rio Branco");
        customer.Address.Number.Should().Be("156");
        customer.Address.Neighborhood.Should().Be("Centro");
        customer.Address.FederativeUnit.Abbreviation.Should().Be("RJ");
        customer.Address.FederativeUnit.State.Should().Be("Rio de Janeiro");
        customer.GetUncommittedEvents().Should().HaveCount(1);
        customer.GetUncommittedEvents().Single().Should().BeOfType<CustomerAddressChanged>();
    }

    [Fact]
    public void UpdateAddress_WithSameAddress_ShouldNotRaiseEvent()
    {
        var customer = CreateCustomer();
        customer.MarkEventsAsCommitted();

        customer.UpdateAddress(
            postalCode: "44073-200",
            street: "Av. Artemia Pires",
            number: "1000",
            neighborhood: "Registro",
            city: "Feira de Santana",
            federativeUnit: "BA");

        customer.GetUncommittedEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateTelephone_WithNewNumber_ShouldRaiseCustomerTelephoneChanged()
    {
        var customer = CreateCustomer();
        customer.MarkEventsAsCommitted();

        customer.UpdateTelephone("71998866554", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        customer.Telephone.Number.Should().Be("71998866554");
        customer.Telephone.FormattedNumber.Should().Be("(71) 99886-6554");
        customer.GetUncommittedEvents().Should().HaveCount(1);
        customer.GetUncommittedEvents().Single().Should().BeOfType<CustomerTelephoneChanged>();
    }

    [Fact]
    public void UpdateAnyInfo_OnInactiveCustomer_ShouldThrowDomainException()
    {
        var customer = CreateCustomer();
        customer.Disable(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        customer.MarkEventsAsCommitted();

        var act = () => customer.UpdateTelephone("71998866554", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        act.Should().Throw<BusinessRuleException>()
            .WithMessage("Operação não permitida para clientes inativos.");
    }
}