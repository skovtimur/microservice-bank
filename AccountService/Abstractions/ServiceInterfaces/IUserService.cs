namespace AccountService.Abstractions.ServiceInterfaces;

public interface IUserService
{
    public Task<bool> IsUserExistAlwaysReturnTrue(Guid userId);
}