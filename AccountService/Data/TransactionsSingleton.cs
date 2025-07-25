using AccountService.Domain.Entities;

namespace AccountService.Data;

public class TransactionsSingleton
{
    public static readonly List<TransactionEntity> Transactions = [];
}