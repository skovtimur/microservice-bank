namespace AccountService.Shared.Abstractions.BackgroundJobInterfaces;

public interface IInterestAccrualDailyBackgroundJob : IRecurringBackgroundJob
{
    public new Task Run();
}