using MediatR;

namespace AccountService.Commands.PartiallyUpdateWallet;

public class PartiallyUpdateWalletCommand(Guid id, Guid ownerId, decimal newInterestRate, DateTime closedAtUtc)
    : IRequest
{
    public Guid Id { get; set; } = id;
    public Guid OwnerId { get; set; } = ownerId;
    public decimal NewInterestRate { get; set; } = newInterestRate;
    public DateTime ClosedAtUtc { get; set; } = closedAtUtc;
}