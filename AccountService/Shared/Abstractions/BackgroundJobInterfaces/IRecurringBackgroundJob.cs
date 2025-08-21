namespace AccountService.Shared.Abstractions.BackgroundJobInterfaces;

public interface IRecurringBackgroundJob
{
    // ReSharper disable once UnusedMember.Global
    public Task Run();
}