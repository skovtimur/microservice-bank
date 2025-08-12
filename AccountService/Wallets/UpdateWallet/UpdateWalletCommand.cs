using AccountService.Shared.Domain;
using AccountService.Wallets.Domain;
using MediatR;

namespace AccountService.Wallets.UpdateWallet;

public class UpdateWalletCommand : IRequest
{
    // ReSharper disable MemberCanBePrivate.Global
    // для решения 2-х подсказок ниже Resharper предлогает сделать конструктор, он мне не нужен тк есть фабричный метод Create()
    public required Guid Id { get; init; }
    public required Guid OwnerId { get; init; }
    public required WalletType NewType { get; init; }
    public required CurrencyValueObject NewCurrency { get; init; }
    public decimal? NewInterestRate { get; init; }
    public required decimal NewBalance { get; init; }
    public DateTime? ClosedAtUtc { get; init; }

    public static MbResult<UpdateWalletCommand> Create(Guid id, Guid ownerId, WalletType newType,
        CurrencyValueObject newCurrency,
        decimal newBalance, decimal? newInterestRate, DateTime? closedAtUtc)
    {
        var wallet = new UpdateWalletCommand
        {
            Id = id,
            OwnerId = ownerId,
            NewType = newType,
            NewCurrency = newCurrency,
            NewBalance = newBalance,
            NewInterestRate = newInterestRate,
            ClosedAtUtc = closedAtUtc
        };
        var result = UpdateWalletCommandValidator.IsValid(wallet);
        
        // ReSharper disable once NullableWarningSuppressionIsUsed
        // раз не валидно значит есть причина почему
        
        return result.IsSuccess
            ? MbResult<UpdateWalletCommand>.Ok(wallet)
            : MbResult<UpdateWalletCommand>.Fail(result.Error!);
    }
}