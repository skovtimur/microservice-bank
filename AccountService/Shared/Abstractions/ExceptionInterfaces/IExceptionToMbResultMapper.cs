using AccountService.Shared.Domain;

namespace AccountService.Shared.Abstractions.ExceptionInterfaces;

public interface IExceptionToMbResultMapper
{
    // ReSharper disable once UnusedMemberInSuper.Global
    // Данный Метод реализуют как минимум 4 класса, без понятия почему Resharper ругался
    public MbResult<T> ToMbResult<T>();
}