using AccountService.DTOs;

namespace AccountService.Abstractions.ExceptionInterfaces;

public interface IExceptionToMbResultMapper
{
    public MbResult<T> ToMbResult<T>();
}