using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Domain.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    private readonly IAppDbContext _context;

    public CreateCustomerValidator(IAppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Telephone)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.PostalCode)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Street)
            .NotNull()
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.AddressNumber)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Neighborhood)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.City)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.FederativeUnit)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.BirthOrFoundationDate)
            .NotNull()
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("A data deve ser anterior à data atual.")
            .Must(BeAtLeast18YearsOld).WithMessage("Pessoa Física deve ter no mínimo 18 anos.")
                .When(x => IsIndividual(x.DocumentNumber));

        RuleFor(x => x.BirthOrFoundationDate)
            .NotNull()
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("A data deve ser anterior à data atual.")
            .When(x => !IsIndividual(x.DocumentNumber));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.StateRegistration))
            .WithMessage("Pessoa Jurídica deve informar a Inscrição Estadual ou marcar como Isento.")
            .When(x => !IsIndividual(x.DocumentNumber));

        RuleFor(x => x.DocumentNumber)
            .NotNull()
            .NotEmpty()
            .MustAsync(BeUniqueDocumentAsync)
            .WithMessage("Já existe um cadastro com este CPF/CNPJ.");

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .MaximumLength(254)
            .MustAsync(BeUniqueEmailAsync)
            .WithMessage("Já existe um cadastro com este e-mail.");
    }

    /// <summary>
    /// Determina se o documento informado é de Pessoa Física (CPF = 11 dígitos).
    /// </summary>
    private static bool IsIndividual(string documentNumber)
    {
        var digits = documentNumber.OnlyDigits();
        return digits.Length == 11 && digits.IsValidCpf();
    }

    /// <summary>
    /// Verifica se a data informada corresponde a uma idade mínima de 18 anos.
    /// </summary>
    private static bool BeAtLeast18YearsOld(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;

        // Ajuste: ainda não fez aniversário no ano corrente
        if (birthDate.AddYears(age) > today)
            age--;

        return age >= 18;
    }

    /// <summary>
    /// Consulta o banco para garantir que não existe outro cliente com o mesmo CPF/CNPJ.
    /// </summary>
    private async Task<bool> BeUniqueDocumentAsync(
        string documentNumber,
        CancellationToken cancellationToken)
    {
        var digits = documentNumber.OnlyDigits();
        return !await _context.Customers.AnyAsync(c => c.Document.Number == digits, cancellationToken);
    }

    /// <summary>
    /// Consulta o banco para garantir que não existe outro cliente com o mesmo e-mail.
    /// </summary>
    private async Task<bool> BeUniqueEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return !await _context.Customers.AnyAsync(c => c.Email.Address == email, cancellationToken);
    }
}