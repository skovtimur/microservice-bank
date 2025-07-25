using AccountService.Commands.UpdateWallet;
using AccountService.Domain;
using FluentValidation;

namespace AccountService.Validators;

public class UpdateWalletCommandValidator : AbstractValidator<UpdateWalletCommand>
{
    public UpdateWalletCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().Must(x => x != Guid.Empty);

        RuleFor(x => x.NewCurrency.Currency).NotEmpty().NotNull();
        RuleFor(x => x.NewType).NotEmpty();

        RuleFor(x => x.NewBalance).Must(x => x >= 0);
        RuleFor(x => x.NewInterestRate).Must(x => x == null || x is >= 0 and <= 100);

        RuleFor(x => x).Must(x =>
            (x.NewType == WalletType.Checking && x.NewInterestRate == null)
            || (x.NewType != WalletType.Checking && x.NewInterestRate != null));

        RuleFor(x => x.ClosedAtUtc).Must(x => x == null || x > DateTime.UtcNow);
    }

    private static readonly UpdateWalletCommandValidator CommandValidator = new();

    public static bool IsValid(UpdateWalletCommand updateWalletCommand) =>
        CommandValidator.Validate(updateWalletCommand).IsValid;
}