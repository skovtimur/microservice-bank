using System.Globalization;

namespace AccountService.Extensions;

public static class StringExtensions
{
    private static readonly HashSet<string> _currencyCodes;

    static StringExtensions()
    {
        _currencyCodes = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(x => !x.Equals(CultureInfo
                .InvariantCulture)) //Remove the invariant culture as a region cannot be created from it.
            .Where(x => !x.IsNeutralCulture) //Remove nuetral cultures as a region cannot be created from them.
            .Select(x => new RegionInfo(x.Name).ISOCurrencySymbol).ToHashSet();
    }

    public static bool IsIsoCurrencyCode(this string str)
    {
        return _currencyCodes.Any(x => x == str);
    }
}