using AccountService.Commands.CreateTransaction;
using AccountService.DTOs;
using FluentValidation;

namespace AccountService.Validators;

public class TransactionCreateCommandValidator : AbstractValidator<TransactionCreateCommand>
{
    public const int MaxDescriptionLength = 5000;

    // ReSharper disable once MemberCanBePrivate.Global
    // the Validators should be public for FluentValidation working
    public TransactionCreateCommandValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().Must(x => x != Guid.Empty).WithMessage("Account Id must be set");
        RuleFor(x => x.OwnerId).NotEmpty().Must(x => x != Guid.Empty).WithMessage("Owner Id must be set");
        RuleFor(x => x.CounterpartyAccountId).Must(x => x != Guid.Empty)
            .WithMessage("Counterparty Account Id must be not Guid.Empty");

        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Description must be set")
            .Must(x => x.Description.Length <= MaxDescriptionLength)
            .WithMessage("Description must be less than 5000 characters");

        RuleFor(x => x.Currency.Currency).NotNull().NotEmpty().WithMessage("Currency must be set");
        RuleFor(x => x.Sum).Must(x => x is > 0 and < int.MaxValue)
            .WithMessage("Sum must be greater than 0 and less than int.MaxValue");
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

        return result.IsValid
            ? MbResult<bool>.Ok(true)
            : MbResult<bool>.Fail(firstError?.ErrorMessage);
    }
}