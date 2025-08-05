using AccountService.Commands.CreateWallet;
using AccountService.Domain;
using AccountService.DTOs;
using FluentValidation;

namespace AccountService.Validators;

public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty().Must(x => x != Guid.Empty).WithMessage("OwnerId must be set");
        RuleFor(x => x.Currency.Currency).NotEmpty().NotNull().WithMessage("Currency must be set");
        RuleFor(x => x.Balance).Must(x => x >= 0).WithMessage("Balance must be greater or equals than 0");
        RuleFor(x => x.InterestRate).Must(x => x == null || x is >= 0 and <= 100)
            .WithMessage("Interest Rate must be greater or equals than 0% and less or equals than 100%");

        RuleFor(x => x).Must(x =>
            (x.Type == WalletType.Checking && x.InterestRate == null)
            || (x.Type != WalletType.Checking && x.InterestRate != null)).WithMessage(
            "If Type is WalletType.Checking then Interest Rate must be null or If Type isn't WalletType.Checking, Interest Rate must be set");

        RuleFor(x => x.ClosedAtUtc).Must(x => x == null || x > DateTime.UtcNow)
            .WithMessage("ClosedAtUtc must be empty or be in the future");
    }

    private static readonly CreateWalletCommandValidator _validator = new()
    {
        ClassLevelCascadeMode = CascadeMode.Continue,
        RuleLevelCascadeMode = CascadeMode.Stop
    };

    public static MbResult<bool> IsValid(CreateWalletCommand newWallet)
    {
        var result = _validator.Validate(newWallet);
        var firstError = result.Errors.FirstOrDefault();

        return result.IsValid
            ? MbResult<bool>.Ok(true)
            : MbResult<bool>.Fail(firstError?.ErrorMessage);
    }
}