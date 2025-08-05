using AccountService.Abstractions.ExceptionInterfaces;
using AccountService.DTOs;

namespace AccountService.Exceptions;

public class BadRequestExсeption : Exception, IExceptionToMbResultMapper
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

    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail(Message);
    }
}