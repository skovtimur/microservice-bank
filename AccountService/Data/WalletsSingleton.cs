using AccountService.Domain.Entities;

namespace AccountService.Data;

public static class WalletsSingleton
{
    public static readonly List<WalletEntity> Wallets = [];
}