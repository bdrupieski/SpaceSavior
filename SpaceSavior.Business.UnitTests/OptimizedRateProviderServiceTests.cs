using SpaceSavior.Business.Services;

namespace SpaceSavior.Business.UnitTests
{
    public class OptimizedRateProviderServiceTests : RateProviderServiceTestsBase
    {
        public OptimizedRateProviderServiceTests() : base(new OptimizedRateProviderService(new RateConfigurationSettings()))
        {
        }
    }
}
