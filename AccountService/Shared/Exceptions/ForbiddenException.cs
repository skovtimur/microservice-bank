using AccountService.Shared.Abstractions.ExceptionInterfaces;
using AccountService.Shared.Domain;

namespace AccountService.Shared.Exceptions;

public class ForbiddenException(string message) : Exception(message), IExceptionToMbResultMapper
{
    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail(new MbError($"Access denied: {Message}", this));
    }
}