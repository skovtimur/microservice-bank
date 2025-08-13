using AccountService.Shared.Abstractions.BackgroundJobInterfaces;
using AccountService.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Shared.Extensions;

public static class WebApplicationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
        dbContext.Database.Migrate();
    }
    
    public static void LaunchRecurrentJobs(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var interestAccrualJob = scope.ServiceProvider.GetRequiredService<IInterestAccrualDailyBackgroundJob>();
        interestAccrualJob.Run();
    }
}