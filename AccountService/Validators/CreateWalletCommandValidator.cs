using AccountService.Commands.CreateWallet;
using AccountService.Domain;
using FluentValidation;

namespace AccountService.Validators;

public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    private CreateWalletCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty().Must(x => x != Guid.Empty);
        RuleFor(x => x.Currency.Currency).NotEmpty().NotNull();
        RuleFor(x => x.Balance).Must(x => x >= 0);
        RuleFor(x => x.InterestRate).Must(x => x == null || x is >= 0 and <= 100);

        RuleFor(x => x).Must(x =>
            (x.Type == WalletType.Checking && x.InterestRate == null)
            || (x.Type != WalletType.Checking && x.InterestRate != null));
        
        RuleFor(x => x.ClosedAtUtc).Must(x => x == null || x > DateTime.UtcNow);
    }

    private static readonly CreateWalletCommandValidator CommandValidator = new();
    public static bool IsValid(CreateWalletCommand newWallet) => CommandValidator.Validate(newWallet).IsValid;
}