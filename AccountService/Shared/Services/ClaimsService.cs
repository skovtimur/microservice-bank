using System.Security.Claims;
using AccountService.Shared.Abstractions.ServiceInterfaces;

namespace AccountService.Shared.Services;

public class ClaimsService : IClaimsService
{
    public bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(id) == false 
            && Guid.TryParse(id, out var guid))
        {
            userId = guid;
            return true;
        }

        userId = Guid.Empty;
        return false;
    }
}