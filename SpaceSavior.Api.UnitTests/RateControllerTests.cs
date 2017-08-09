using System;
using Moq;
using SpaceSavior.Api.Controllers;
using SpaceSavior.Business.Services;
using Xunit;

namespace SpaceSavior.Api.UnitTests
{
    /// <summary>
    /// The test names in this class follow the convention
    /// MethodName_StateUnderTest_ExpectedBehavior
    /// </summary>
    public class RateControllerTests
    {
        private readonly RateController _rateController;
        private readonly Mock<IRateProviderService> _rateProviderService;

        public RateControllerTests()
        {
            _rateProviderService = new Mock<IRateProviderService>();
            _rateController = new RateController(_rateProviderService.Object);
        }

        [Fact]
        public void GetRateQuote_TimeFormatWithTimeZone_UsesCorrectlyParsedDates()
        {
            var expectedFrom = new DateTimeOffset(2017, 1, 1, 8, 15, 30, TimeSpan.FromHours(-5));
            var expectedTo = new DateTimeOffset(2017, 1, 2, 10, 59, 00, TimeSpan.FromHours(10));
            _rateController.GetRateQuote("2017-01-01T08:15:30-05:00", "2017-01-02T10:59:00+10:00");
            _rateProviderService.Verify(x => x.GetPrice(
                    It.Is<DateTimeOffset>(y => y == expectedFrom),
                    It.Is<DateTimeOffset>(y => y == expectedTo)),
                Times.Once);
        }

        [Fact]
        public void GetRateQuote_TimeFormatWithZ_UsesCorrectlyParsedDates()
        {
            var expectedFrom = new DateTimeOffset(2017, 1, 1, 12, 10, 45, TimeSpan.FromHours(0));
            var expectedTo = new DateTimeOffset(2017, 12, 12, 23, 59, 59, TimeSpan.FromHours(0));
            _rateController.GetRateQuote("2017-01-01T12:10:45Z", "2017-12-12T23:59:59Z");
            _rateProviderService.Verify(x => x.GetPrice(
                    It.Is<DateTimeOffset>(y => y == expectedFrom),
                    It.Is<DateTimeOffset>(y => y == expectedTo)),
                Times.Once);
        }
    }
}
