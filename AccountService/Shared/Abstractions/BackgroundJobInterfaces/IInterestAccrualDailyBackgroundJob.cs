namespace AccountService.Shared.Abstractions.BackgroundJobInterfaces;

public interface IInterestAccrualDailyBackgroundJob : IRecurringBackgroundJob
{
    public new void Run();
}