using System.ComponentModel.DataAnnotations;
using AccountService.Domain;
using AccountService.Requests;

namespace AccountService.Filters;

[AttributeUsage(AttributeTargets.Class)]
public class InterestRateRequiredCheckingTypeFilter : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        decimal? interestRate = null;
        WalletType type = WalletType.Checking;

        if (value is not WalletCreateRequest or WalletUpdateRequest)
            return ValidationResult.Success;

        if (value is WalletCreateRequest createWalletRequest)
        {
            interestRate = createWalletRequest.InterestRate;
            type = createWalletRequest.Type;
        }

        if (value is WalletUpdateRequest updateWalletRequest)
        {
            interestRate = updateWalletRequest.NewInterestRate;
            type = updateWalletRequest.NewType;
        }

        if ((type == WalletType.Checking && interestRate == null)
            || (type != WalletType.Checking && interestRate != null))
            return ValidationResult.Success;

        return new ValidationResult(
            $"Only Wallet with {WalletType.Deposit} or {WalletType.Credit} type can have an InterestRate and in this case it should be not null");
    }
}