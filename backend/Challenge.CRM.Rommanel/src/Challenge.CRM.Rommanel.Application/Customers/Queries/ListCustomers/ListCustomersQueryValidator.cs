using FluentValidation;

namespace Challenge.CRM.Rommanel.Application.Customers.Queries.ListCustomers;

public sealed class ListCustomersQueryValidator : AbstractValidator<ListCustomersQuery>
{
    private const int MinPage = 1;
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;
    private const int MaxSearchLength = 100;

    public ListCustomersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(MinPage)
            .WithMessage($"A página deve ser maior ou igual a {MinPage}.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(MinPageSize, MaxPageSize)
            .WithMessage($"O tamanho da página deve ser entre {MinPageSize} e {MaxPageSize}.");

        RuleFor(x => x.Search)
            .MaximumLength(MaxSearchLength)
            .WithMessage($"O termo de busca deve ter no máximo {MaxSearchLength} caracteres.")
            .When(x => x.Search is not null);
    }
}