namespace AccountService.Exceptions;

public class BadRequestExсeption : Exception
{
    public BadRequestExсeption(string text) : base(text)
    {
    }

    public BadRequestExсeption(Type type, string propertyName) : base($"{type}.{propertyName} was invalid")
    {
    }

    public BadRequestExсeption(Type type, string propertyName, string toBeValidText) : base(
        $"{type}.{propertyName} was invalid. To be Valid it need: {toBeValidText}")
    {
    }
}