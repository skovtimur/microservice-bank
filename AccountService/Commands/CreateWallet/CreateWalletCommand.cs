using AccountService.Domain;
using AccountService.Domain.ValueObjects;
using AccountService.Validators;
using MediatR;

namespace AccountService.Commands.CreateWallet;

public class CreateWalletCommand : IRequest<Guid>
{
    public Guid OwnerId { get; set; }

    public WalletType Type { get; set; }
    public CurrencyValueObject Currency { get; set; }
    public decimal? InterestRate { get; set; }
    public decimal Balance { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public static CreateWalletCommand? Create(Guid ownerId, WalletType type,
        CurrencyValueObject currency, decimal balance, decimal? interestRate, DateTime? closedAtUtc)
    {
        var wallet = new CreateWalletCommand
        {
            OwnerId = ownerId,
            Type = type,
            Currency = currency,
            InterestRate = interestRate,
            Balance = balance,
            ClosedAtUtc = closedAtUtc
        };

        return CreateWalletCommandValidator.IsValid(wallet) ? wallet : null;
    }
}