using AccountService.Domain.Entities;

namespace AccountService.Data;

public static class TransactionsSingleton
{
    public static readonly List<TransactionEntity> Transactions = [];
}