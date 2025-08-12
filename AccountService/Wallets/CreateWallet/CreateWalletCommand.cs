using AccountService.Shared.Domain;
using AccountService.Wallets.Domain;
using MediatR;

namespace AccountService.Wallets.CreateWallet;

public class CreateWalletCommand : IRequest<Guid>
{
    // ReSharper disable MemberCanBePrivate.Global
    // для решения 2-х подсказок ниже Resharper предлогает сделать конструктор, он мне не нужен тк есть фабричный метод Create()
    public required Guid OwnerId { get; init; }
    public required WalletType Type { get; init; }
    public required CurrencyValueObject Currency { get; init; }
    public decimal? InterestRate { get; init; }
    public required decimal Balance { get; init; }
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

        // ReSharper disable once NullableWarningSuppressionIsUsed
        // раз не валидно значит есть причина почему
        
        return result.IsSuccess
            ? MbResult<CreateWalletCommand>.Ok(wallet)
            : MbResult<CreateWalletCommand>.Fail(result.Error!);
    }
}