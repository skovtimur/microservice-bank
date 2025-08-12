using AccountService.Shared.Exceptions;
using AccountService.Transactions.Domain;
using AutoMapper;
using MediatR;

namespace AccountService.Transactions.GetTransaction;

public class GetTransactionQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    : IRequestHandler<GetTransactionQuery, TransactionDto?>
{
    public async Task<TransactionDto?> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.Get(request.Id);

        if (transaction is null)
            throw new NotFoundException(typeof(TransactionEntity), request.Id);

        if (transaction.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You can't see these transactions because you aren't owner");

        var dto = mapper.Map<TransactionDto>(transaction);
        return dto;
    }
}