using AccountService.Features.Wallets.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Shared.Infrastructure.Repositories;

public class WalletRepository(MainDbContext dbContext, IMapper mapper) : IWalletRepository
{
    public async Task<WalletEntity?> Get(Guid id)
    {
        var foundWallet = await dbContext.Wallets
            .Include(x => x.Transactions)
            .FirstOrDefaultAsync(x => x.Id == id);

        return foundWallet;
    }

    public async Task<WalletEntity?> GetWithReload(Guid id)
    {
        var foundWallet = await Get(id);

        if (foundWallet != null)
        {
            await dbContext.Entry(foundWallet).ReloadAsync();
            foundWallet = await Get(id);
        }

        return foundWallet;
    }

    public async Task<List<WalletDto>> GetAllWalletByUserId(Guid userId)
    {
        var wallets = await dbContext.Wallets
            .Include(x => x.Transactions)
            .Where(x => x.OwnerId == userId)
            .Where(x => x.IsDeleted == false)
            .Select(x => mapper.Map<WalletDto>(x))
            .ToListAsync();

        return wallets;
    }

    public async Task Create(WalletEntity newWallet)
    {
        await dbContext.Wallets.AddAsync(newWallet);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(WalletEntity updatedWallet)
    {
        updatedWallet.UpdateEntity();
        dbContext.Wallets.Update(updatedWallet);

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(WalletEntity deletedWallet)
    {
        deletedWallet.DeleteEntitySoftly();
        await dbContext.SaveChangesAsync();
    }

    public async Task FreezeByOwnerId(Guid ownerId)
    {
        var wallets = await dbContext.Wallets
            .Where(x => x.OwnerId == ownerId)
            .ToListAsync();

        foreach (var w in wallets)
            w.Freeze();

        await dbContext.SaveChangesAsync();
    }

    public async Task UnFreezeByOwnerId(Guid ownerId)
    {
        var wallets = await dbContext.Wallets
            .Where(x => x.OwnerId == ownerId)
            .ToListAsync();

        foreach (var w in wallets)
            w.Unfreeze();

        await dbContext.SaveChangesAsync();
    }
}