using AccountService.Abstractions.ExceptionInterfaces;
using AccountService.DTOs;

namespace AccountService.Exceptions;

public class ForbiddenException(string message) : Exception(message), IExceptionToMbResultMapper
{
    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail($"Access denied: {Message}");
    }
}