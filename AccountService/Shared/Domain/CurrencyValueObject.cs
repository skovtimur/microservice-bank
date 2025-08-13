using AccountService.Shared.Extensions;

namespace AccountService.Shared.Domain;

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

    public override string ToString()
    {
        return Currency;
    }
}