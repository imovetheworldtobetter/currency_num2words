/*
 * Purpose: Defines shared HTTP header names and default client identity values.
*/

namespace myCurrencyMagic.Shared.Contracts;

public static class ApiHeaders
{
    public const string ClientHeaderName = "X-myCurrencyMagic-Client";
    public const string DefaultClientHeaderValue = "myCurrencyMagic.Client";
}
