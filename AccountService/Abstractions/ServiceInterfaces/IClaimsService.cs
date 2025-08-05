using System.Security.Claims;

namespace AccountService.Abstractions.ServiceInterfaces;

public interface IClaimsService
{
    public bool TryGetUserId(ClaimsPrincipal principal, out Guid userId);
}