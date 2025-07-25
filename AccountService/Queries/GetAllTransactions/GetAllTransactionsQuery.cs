using AccountService.Domain.Entities;
using AccountService.DTOs;
using MediatR;

namespace AccountService.Queries.GetAllTransactions;

public class GetAllTransactionsQuery(Guid accountId, DateTime fromAtUtc) : IRequest<List<TransactionDto>>
{
    public Guid AccountId { get; set; } = accountId;
    public DateTime FromAtUtc { get; set; } = fromAtUtc;
}