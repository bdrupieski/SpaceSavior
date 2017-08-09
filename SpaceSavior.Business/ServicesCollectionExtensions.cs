using System;
using Microsoft.Extensions.DependencyInjection;
using SpaceSavior.Business.Services;

namespace SpaceSavior.Business
{
    public static class ServicesCollectionExtensions
    {
        public static void AddSpaceSaviorServices(this IServiceCollection serviceCollection, IRateConfigurationSettings rateConfigurationSettings)
        {
            var rateProviderService = new RateProviderService(rateConfigurationSettings);
            bool ratesSuccessfullySet = rateProviderService.SetUpConfiguredDefaultRates();
            if (!ratesSuccessfullySet)
            {
                throw new InvalidOperationException("The default rates JSON file is invalid.");
            }

            serviceCollection.AddSingleton(rateConfigurationSettings);
            serviceCollection.AddSingleton<IRateProviderService>(rateProviderService);
        }
    }
}
