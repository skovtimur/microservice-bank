namespace AccountService.Shared.Extensions;

public static class WebHostEnvironmentExtensions
{
    public static bool IsTestEnvironment(this IWebHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment("Test");
    }
}