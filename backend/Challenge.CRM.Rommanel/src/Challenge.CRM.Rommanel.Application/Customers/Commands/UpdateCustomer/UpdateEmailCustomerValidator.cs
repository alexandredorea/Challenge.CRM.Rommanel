using Challenge.CRM.Rommanel.Application.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer
{
    public sealed class UpdateEmailCustomerValidator : AbstractValidator<UpdateEmailCustomerCommand>
    {
        private readonly IAppDbContext _context;

        public UpdateEmailCustomerValidator(IAppDbContext context)
        {
            _context = context;

            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .MaximumLength(254)
                .MustAsync(BeUniqueEmailAsync)
                    .WithMessage("Já existe um cadastro com este e-mail.");
        }

        private async Task<bool> BeUniqueEmailAsync(
            UpdateEmailCustomerCommand command,
            string email,
            CancellationToken cancellationToken)
        {
            return !await _context.Customers
                .AnyAsync(c => c.Email.Address == email/* && c.Id != command.CustomerId*/,
                cancellationToken);
        }
    }
}