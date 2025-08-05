using MediatR;

namespace AccountService.Queries.HasWalletBeenUsed;

public class HasWalletBeenUsedQuery(Guid id) : IRequest<bool>
{
    public Guid Id { get; } = id;
}