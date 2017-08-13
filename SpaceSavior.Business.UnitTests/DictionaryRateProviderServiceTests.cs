using SpaceSavior.Business.Services;

namespace SpaceSavior.Business.UnitTests
{
    public class DictionaryRateProviderServiceTests : RateProviderServiceTestsBase
    {
        public DictionaryRateProviderServiceTests() : base(new DictionaryRateProviderService(new RateConfigurationSettings()))
        {
        }
    }
}
