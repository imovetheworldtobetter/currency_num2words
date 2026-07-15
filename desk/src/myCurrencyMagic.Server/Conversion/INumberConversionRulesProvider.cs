/*
 * Purpose: Provides loaded number conversion rules to the conversion service.
*/

namespace myCurrencyMagic.Server.Conversion;

public interface INumberConversionRulesProvider
{
    NumberConversionRules GetRules();
}
