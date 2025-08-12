using AccountService.Shared.Abstractions.ExceptionInterfaces;
using AccountService.Shared.Domain;

namespace AccountService.Shared.Exceptions;

public class NotFoundException : Exception, IExceptionToMbResultMapper
{
    public NotFoundException(string str) : base(str)
    {
    }

    public NotFoundException(Type objectType, Guid id) : base($"Object:{objectType} with ID={id} was not found")
    {
    }

    public MbResult<T> ToMbResult<T>()
    {
        return MbResult<T>.Fail(new MbError(Message, this));
    }
}