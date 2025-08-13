using AccountService.Shared.Mapper.Converters;
using AccountService.Transactions.CreateTransaction;
using AccountService.Transactions.Domain;
using AccountService.Wallets.CreateWallet;
using AccountService.Wallets.Domain;
using AutoMapper;

namespace AccountService.Shared.Mapper;

public class MainMapper : Profile
{
    public MainMapper()
    {
        // ReSharper disable SpecifyACultureInStringConversionExplicitly

        CreateMap<CreateWalletCommand, WalletEntity>();
        CreateMap<TransactionCreateCommand, TransactionEntity>();

        CreateMap<TransactionEntity, TransactionDto>()
            .ForMember(x => x.DeletedAtUtc,
                opt => opt.MapFrom(y => y.DeletedAtUtc == null ? null : y.DeletedAtUtc.ToString()))
            .ForMember(x => x.CreatedAtUtc, opt => opt.MapFrom(y => y.CreatedAtUtc.ToString()))
            .ForMember(x => x.TransactionType, x => x.MapFrom(y => y.TransactionType.ToString()))
            .ForMember(x => x.Description, x => x.MapFrom(y => y.Description.ToString()))
            .ForMember(x => x.OwnerId, x => x.MapFrom(y => y.OwnerId))
            .ForMember(x => x.Currency, x => x.MapFrom(y => y.Currency.ToString()));

        CreateMap<WalletEntity, WalletDto>()
            .ForMember(x => x.ClosedAtUtc,
                opt => opt.MapFrom(y => y.ClosedAtUtc == null ? null : y.ClosedAtUtc.ToString()))
            .ForMember(x => x.DeletedAtUtc,
                opt => opt.MapFrom(y => y.DeletedAtUtc == null ? null : y.DeletedAtUtc.ToString()))
            .ForMember(x => x.UpdatedAtUtc,
                opt => opt.MapFrom(y => y.UpdatedAtUtc == null ? null : y.UpdatedAtUtc.ToString()))
            .ForMember(x => x.OpenedAtUtc, opt => opt.MapFrom(y => y.OpenedAtUtc.ToString()))
            .ForMember(x => x.CreatedAtUtc, opt => opt.MapFrom(y => y.CreatedAtUtc.ToString()))
            .ForMember(x => x.Type, x => x.MapFrom(y => y.Type.ToString()))
            .ForMember(x => x.Currency, x => x.MapFrom(y => y.Currency.ToString()))
            .ForMember(x => x.Transactions, opt =>
                opt.ConvertUsing(new TransactionListConverter(), src => src.Transactions));
    }
}