using MediatR;

namespace AccountService.Features.Wallets.PartiallyUpdateWallet;

public class PartiallyUpdateWalletCommand(Guid id, Guid ownerId, decimal newInterestRate, DateTime closedAtUtc)
    : IRequest
{
    public Guid Id { get; } = id;
    public Guid OwnerId { get; } = ownerId;
    public decimal NewInterestRate { get; } = newInterestRate;
    public DateTime ClosedAtUtc { get; } = closedAtUtc;
}