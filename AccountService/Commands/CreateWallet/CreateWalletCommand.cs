using AccountService.Domain;
using AccountService.Domain.ValueObjects;
using AccountService.DTOs;
using AccountService.Validators;
using MediatR;

namespace AccountService.Commands.CreateWallet;

public class CreateWalletCommand : IRequest<Guid>
{
    public Guid OwnerId { get; init; }
    public WalletType Type { get; init; }
    public CurrencyValueObject Currency { get; init; }
    public decimal? InterestRate { get; init; }
    public decimal Balance { get; init; }
    public DateTime? ClosedAtUtc { get; init; }

    public static MbResult<CreateWalletCommand> Create(Guid ownerId, WalletType type,
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
        var result = CreateWalletCommandValidator.IsValid(wallet);

        return result.IsSuccess
            ? MbResult<CreateWalletCommand>.Ok(wallet)
            : MbResult<CreateWalletCommand>.Fail(result.ErrorMessage);
    }
}