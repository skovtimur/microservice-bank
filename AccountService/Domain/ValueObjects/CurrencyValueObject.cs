using AccountService.Extensions;

namespace AccountService.Domain.ValueObjects;

public class CurrencyValueObject
{
    public required string Currency { get; init; }

    public static CurrencyValueObject? Create(string isoCurrencyCode)
    {
        isoCurrencyCode = isoCurrencyCode.ToUpper();

        return isoCurrencyCode.IsIsoCurrencyCode()
            ? new CurrencyValueObject { Currency = isoCurrencyCode }
            : null;
    }
}