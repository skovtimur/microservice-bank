using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Domain;
using FluentValidation;

namespace AccountService.Features.Wallets.UpdateWallet;

public class UpdateWalletCommandValidator : AbstractValidator<UpdateWalletCommand>
{
    // ReSharper disable once MemberCanBePrivate.Global
    // the Validators should be public for FluentValidation working
    public UpdateWalletCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().Must(x => x != Guid.Empty).WithMessage("Id must be set");

        RuleFor(x => x.NewCurrency.Currency).NotEmpty().NotNull().WithMessage("New Currency must be set");
        RuleFor(x => x.NewBalance).Must(x => x >= 0).WithMessage("New Balance must be greater or equals than 0");
        
        // ReSharper disable once RedundantPatternParentheses
        // думаю скобки внутри которых >= 0 and <= 100 будут делать код более читабельным как бы закрывая от is null условия  
        RuleFor(x => x.NewInterestRate).Must(x => x is null or (>= 0 and <= 100))
            .WithMessage("New Interest Rate must be greater or equals than 0% and less or equals than 100%");

        RuleFor(x => x).Must(x =>
                (x.NewType == WalletType.Checking && x.NewInterestRate == null)
                || (x.NewType != WalletType.Checking && x.NewInterestRate != null))
            .WithMessage(
                "If New Type is WalletType.Checking then Interest Rate must be null or If New Type isn't WalletType.Checking, Interest Rate must be set");

        RuleFor(x => x.ClosedAtUtc).Must(x => x == null || x > DateTime.UtcNow)
            .WithMessage("New ClosedAtUtc must be empty or be in the future");
    }

    private static readonly UpdateWalletCommandValidator Validator = new()
    {
        ClassLevelCascadeMode = CascadeMode.Continue,
        RuleLevelCascadeMode = CascadeMode.Stop
    };

    public static MbResult<bool> IsValid(UpdateWalletCommand updateWalletCommand)
    {
        var result = Validator.Validate(updateWalletCommand);
        var firstError = result.Errors.FirstOrDefault();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        // раз не валидно значит есть причина почему
        
        return result.IsValid
            ? MbResult<bool>.Ok(true)
            : MbResult<bool>.Fail(new MbError(firstError?.ErrorMessage!));
    }
}