using Hangfire.Dashboard;

namespace AccountService.Shared.Api.Filters;

public class AllowAllHangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}