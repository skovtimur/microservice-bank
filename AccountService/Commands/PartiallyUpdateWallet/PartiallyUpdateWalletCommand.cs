using MediatR;

namespace AccountService.Commands.PartiallyUpdateWallet;

public class PartiallyUpdateWalletCommand(Guid id, decimal newInterestRate, DateTime closedAtUtc) : IRequest
{
    public Guid Id { get; set; } = id;
    public decimal NewInterestRate { get; set; } = newInterestRate;
    public DateTime ClosedAtUtc { get; set; } = closedAtUtc;
}