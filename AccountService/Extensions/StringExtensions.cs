using System.Globalization;

namespace AccountService.Extensions;

public static class StringExtensions
{
    private static readonly HashSet<string> CurrencyCodes;

    static StringExtensions()
    {
        CurrencyCodes = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(x => !x.Equals(CultureInfo
                .InvariantCulture)) //Remove the invariant culture as a region cannot be created from it.
            .Where(x => !x.IsNeutralCulture) //Remove neutral cultures as a region cannot be created from them.
            .Select(x => new RegionInfo(x.Name).ISOCurrencySymbol).ToHashSet();
    }

    public static bool IsIsoCurrencyCode(this string str)
    {
        return CurrencyCodes.Any(x => x == str);
    }

    public static string GetRequiredString(string? str)
    {
        if (string.IsNullOrEmpty(str))
            throw new ArgumentNullException(nameof(str));

        return str;
    }
}