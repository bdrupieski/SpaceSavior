using SpaceSavior.Business.Services;

namespace SpaceSavior.Business.UnitTests
{
    public class RateProviderServiceTests : RateProviderServiceTestsBase
    {
        public RateProviderServiceTests() : base(new RateProviderService(new RateConfigurationSettings()))
        {
        }
    }
}
