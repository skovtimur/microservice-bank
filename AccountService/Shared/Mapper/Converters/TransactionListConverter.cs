using AccountService.Features.Transactions.Domain;
using AutoMapper;

namespace AccountService.Shared.Mapper.Converters;

public class TransactionListConverter : IValueConverter<List<TransactionEntity>, List<TransactionDto>>
{
    public List<TransactionDto> Convert(List<TransactionEntity> sourceMember, ResolutionContext context)
    {
        return sourceMember.Select(member => context.Mapper.Map<TransactionDto>(member)).ToList();
    }
}