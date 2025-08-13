using AccountService.Shared.Abstractions.ExceptionInterfaces;
using AccountService.Shared.Domain;

namespace AccountService.Shared.Exceptions;

public class PaymentRequiredException(string message) : Exception(message), IExceptionToMbResultMapper
{
    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail(new MbError($"Payment Required: {Message}", this));
    }
}