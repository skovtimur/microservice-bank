using AccountService.Commands.CreateTransaction;
using AccountService.Commands.CreateWallet;
using AccountService.Domain.Entities;
using AccountService.DTOs;
using AutoMapper;

namespace AccountService;

public class MainMapper : Profile
{
    public MainMapper()
    {
        CreateMap<CreateWalletCommand, WalletEntity>();
        CreateMap<TransactionCreateCommand, TransactionEntity>();

        CreateMap<WalletEntity, WalletDto>()
            .ForMember(x => x.ClosedAtUtc,
                opt => opt.MapFrom(y => y.ClosedAtUtc == null ? null : y.ClosedAtUtc.ToString()))
            .ForMember(x => x.DeletedAtUtc,
                opt => opt.MapFrom(y => y.DeletedAtUtc == null ? null : y.DeletedAtUtc.ToString()))
            .ForMember(x => x.UpdatedAtUtc,
                opt => opt.MapFrom(y => y.UpdatedAtUtc == null ? null : y.UpdatedAtUtc.ToString()))

            // ReSharper disable SpecifyACultureInStringConversionExplicitly
            .ForMember(x => x.OpenedAtUtc, opt => opt.MapFrom(y => y.OpenedAtUtc.ToString()))
            .ForMember(x => x.CreatedAtUtc, opt => opt.MapFrom(y => y.CreatedAtUtc.ToString()))
            .ForMember(x => x.Type, x => x.MapFrom(y => y.Type.ToString()))
            .ForMember(x => x.Currency, x => x.MapFrom(y => y.Currency.Currency));

        CreateMap<TransactionEntity, TransactionDto>()
            .ForMember(x => x.DeletedAtUtc,
                opt => opt.MapFrom(y => y.DeletedAtUtc == null ? null : y.DeletedAtUtc.ToString()))
            .ForMember(x => x.CreatedAtUtc, opt => opt.MapFrom(y => y.CreatedAtUtc.ToString()))
            .ForMember(x => x.TransactionType, x => x.MapFrom(y => y.TransactionType.ToString()))
            .ForMember(x => x.Description, x => x.MapFrom(y => y.Description.Description))
            .ForMember(x => x.Currency, x => x.MapFrom(y => y.Currency.Currency));
    }
}