using System.Security.Claims;

namespace AccountService.Shared.Abstractions.ServiceInterfaces;

public interface IClaimsService
{
    public bool TryGetUserId(ClaimsPrincipal principal, out Guid userId);
}