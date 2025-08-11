using AccountService.Abstractions.ExceptionInterfaces;
using AccountService.DTOs;

namespace AccountService.Exceptions;

public class BadRequestException(string text) : Exception(text), IExceptionToMbResultMapper
{
    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail(Message);
    }
}