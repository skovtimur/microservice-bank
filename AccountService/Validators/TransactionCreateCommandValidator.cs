using AccountService.Commands.CreateTransaction;
using FluentValidation;

namespace AccountService.Validators;

public class TransactionCreateCommandValidator : AbstractValidator<TransactionCreateCommand>
{
    private TransactionCreateCommandValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().Must(x => x != Guid.Empty);
        RuleFor(x => x.OwnerId).NotEmpty().Must(x => x != Guid.Empty);
        RuleFor(x => x.CounterpartyAccountId).Must(x => x != Guid.Empty);
        
        RuleFor(x => x.Description).NotNull().NotEmpty()
            .Must(x => x.Description.Length <= 5000);

        RuleFor(x => x.Currency.Currency).NotNull().NotEmpty();
        RuleFor(x => x.Sum).Must(x => x is > 0 and < int.MaxValue);
    }

    private static readonly TransactionCreateCommandValidator _validator = new();
    public static bool IsValid(TransactionCreateCommand command) => _validator.Validate(command).IsValid;
}