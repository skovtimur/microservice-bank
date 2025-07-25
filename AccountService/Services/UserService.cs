using AccountService.Abstractions.ServiceInterfaces;

namespace AccountService.Services;

public class UserService : IUserService
{
    public async Task<bool> IsUserExistAlwaysReturnTrue(Guid userId)
    {
        // TODO 
        // Как будем создавать аутентификацию
        // Так сразу можно создавать и юзеров, поэтому пока он возвращает всегдд true
        
        return true;
    }
}