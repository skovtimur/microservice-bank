using AccountService.Shared.Domain;
using FluentValidation;

namespace AccountService.Features.Transactions.CreateTransaction;

public class TransactionCreateCommandValidator : AbstractValidator<TransactionCreateCommand>
{
    // ReSharper disable once MemberCanBePrivate.Global
    // the Validators should be public for FluentValidation working
    public TransactionCreateCommandValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().Must(x => x != Guid.Empty).WithMessage("Account Id must be set");
        RuleFor(x => x.OwnerId).NotEmpty().Must(x => x != Guid.Empty).WithMessage("Owner Id must be set");
        RuleFor(x => x.CounterpartyAccountId).Must(x => x != Guid.Empty)
            .WithMessage("Counterparty Account Id must be not Guid.Empty");

        RuleFor(x => x.Currency.Currency).NotNull().NotEmpty().WithMessage("Currency must be set");
        RuleFor(x => x.Sum).Must(x => x is > 0 and < int.MaxValue)
            .WithMessage("Sum must be greater than 0 and less than int.MaxValue");

        RuleFor(x => x.Description).SetValidator(new DescriptionValidator());
    }

    private static readonly TransactionCreateCommandValidator Validator = new()
    {
        ClassLevelCascadeMode = CascadeMode.Continue,
        RuleLevelCascadeMode = CascadeMode.Stop
    };

    public static MbResult<bool> IsValid(TransactionCreateCommand command)
    {
        var result = Validator.Validate(command);
        var firstError = result.Errors.FirstOrDefault();
        
        // ReSharper disable once NullableWarningSuppressionIsUsed
        // раз не валидно значит есть причина почему
        
        return result.IsValid
            ? MbResult<bool>.Ok(true)
            : MbResult<bool>.Fail(new MbError(firstError?.ErrorMessage!));
    }
}