namespace AccountService.Features.Wallets.Domain;

public interface IWalletRepository
{
    public Task<WalletEntity?> Get(Guid id);
    public Task<WalletEntity?> GetWithReload(Guid id);
    public Task<List<WalletDto>> GetAllWalletByUserId(Guid userId);
    public Task Create(WalletEntity newWallet);
    public Task Update(WalletEntity updatedWallet);
    public Task Delete(WalletEntity deletedWallet);
    
    public Task FreezeByOwnerId(Guid ownerId);
    public Task UnFreezeByOwnerId(Guid ownerId);
}