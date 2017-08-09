using System;
using SpaceSavior.Business.Models;

namespace SpaceSavior.Business.Services
{
    public interface IRateProviderService
    {
        /// <summary>
        /// Retrieves a price if the given date range is inside a rate definition.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Price for the time range if available, otherwise null if no price is available</returns>
        int? GetPrice(DateTimeOffset from, DateTimeOffset to);

        /// <summary>
        /// Configures the rate provider with the rates from the configured default rates JSON file.
        /// </summary>
        /// <returns>True if the configured rates are valid and were set.</returns>
        bool SetUpConfiguredDefaultRates();

        /// <summary>
        /// Uses the rates defined in the input set of rates.
        /// </summary>
        /// <param name="rateSetInput"></param>
        /// <returns>Approximate time the rates changed, otherwise null if they were not changed</returns>
        DateTimeOffset? UseRates(RateDefinitions rateSetInput);
    }
}
