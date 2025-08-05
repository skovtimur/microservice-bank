using AccountService.Abstractions.ExceptionInterfaces;
using AccountService.DTOs;

namespace AccountService.Exceptions;

public class PaymentRequiredException(string message) : Exception(message), IExceptionToMbResultMapper
{
    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail($"Payment Required: {Message}");
    }
}