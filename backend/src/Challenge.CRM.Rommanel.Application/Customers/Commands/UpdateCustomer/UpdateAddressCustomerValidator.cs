using FluentValidation;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateAddressCustomerValidator : AbstractValidator<UpdateAddressCustomerCommand>
{
    public UpdateAddressCustomerValidator()
    {
        RuleFor(x => x.PostalCode)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Street)
            .NotNull()
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.AddressNumber)
            .NotNull()
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Neighborhood)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.City)
            .NotNull()
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FederativeUnit)
            .NotNull()
            .NotEmpty();
    }
}