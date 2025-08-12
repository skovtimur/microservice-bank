using AccountService.Shared.Abstractions.ExceptionInterfaces;
using AccountService.Shared.Domain;

namespace AccountService.Shared.Exceptions;

public class BadRequestException(string text) : Exception(text), IExceptionToMbResultMapper
{
    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail(new MbError(Message, this));
    }
}