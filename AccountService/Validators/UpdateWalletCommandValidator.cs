using AccountService.Commands.UpdateWallet;
using AccountService.Domain;
using AccountService.DTOs;
using FluentValidation;

namespace AccountService.Validators;

public class UpdateWalletCommandValidator : AbstractValidator<UpdateWalletCommand>
{
    public UpdateWalletCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().Must(x => x != Guid.Empty).WithMessage("Id must be set");

        RuleFor(x => x.NewCurrency.Currency).NotEmpty().NotNull().WithMessage("New Currency must be set");
        RuleFor(x => x.NewBalance).Must(x => x >= 0).WithMessage("New Balance must be greater or equals than 0");
        RuleFor(x => x.NewInterestRate).Must(x => x == null || x is >= 0 and <= 100)
            .WithMessage("New Interest Rate must be greater or equals than 0% and less or equals than 100%");

        RuleFor(x => x).Must(x =>
                (x.NewType == WalletType.Checking && x.NewInterestRate == null)
                || (x.NewType != WalletType.Checking && x.NewInterestRate != null))
            .WithMessage(
                "If New Type is WalletType.Checking then Interest Rate must be null or If New Type isn't WalletType.Checking, Interest Rate must be set");

        RuleFor(x => x.ClosedAtUtc).Must(x => x == null || x > DateTime.UtcNow)
            .WithMessage("New ClosedAtUtc must be empty or be in the future");
    }

    private static readonly UpdateWalletCommandValidator _validator = new()
    {
        ClassLevelCascadeMode = CascadeMode.Continue,
        RuleLevelCascadeMode = CascadeMode.Stop
    };

    public static MbResult<bool> IsValid(UpdateWalletCommand updateWalletCommand)
    {
        var result = _validator.Validate(updateWalletCommand);
        var firstError = result.Errors.FirstOrDefault();

        return result.IsValid
            ? MbResult<bool>.Ok(true)
            : MbResult<bool>.Fail(firstError?.ErrorMessage);
    }
}