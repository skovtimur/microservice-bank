using AccountService.Domain;
using AccountService.Domain.ValueObjects;
using AccountService.Validators;
using MediatR;

namespace AccountService.Commands.UpdateWallet;

public class UpdateWalletCommand : IRequest
{
    //Обновы не должно быть если уже счет имеет транзакции какие нибудь

    public Guid Id { get; set; }
    public WalletType NewType { get; set; }
    public CurrencyValueObject NewCurrency { get; set; }
    public decimal? NewInterestRate { get; set; }
    public decimal NewBalance { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public static UpdateWalletCommand? Create(Guid id, WalletType newType, CurrencyValueObject newCurrency,
        decimal newBalance, decimal? newInterestRate, DateTime? closedAtUtc)
    {
        var wallet = new UpdateWalletCommand
        {
            Id = id,
            NewType = newType,
            NewCurrency = newCurrency,
            NewBalance = newBalance,
            NewInterestRate = newInterestRate,
            ClosedAtUtc = closedAtUtc
        };
        return UpdateWalletCommandValidator.IsValid(wallet) ? wallet : null;
    }
}