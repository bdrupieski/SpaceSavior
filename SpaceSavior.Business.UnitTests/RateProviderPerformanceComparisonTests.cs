using System;
using System.Diagnostics;
using System.Linq;
using SpaceSavior.Business.Models;
using SpaceSavior.Business.Services;
using Xunit;
using Xunit.Abstractions;

namespace SpaceSavior.Business.UnitTests
{
    public class RateProviderPerformanceComparisonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly DictionaryRateProviderService _dictionaryRateProviderService;
        private readonly RateProviderService _rateProviderService;

        public RateProviderPerformanceComparisonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _dictionaryRateProviderService = new DictionaryRateProviderService(new RateConfigurationSettings());
            _rateProviderService = new RateProviderService(new RateConfigurationSettings());

            SetUpRates(_dictionaryRateProviderService);
            SetUpRates(_rateProviderService);
        }

        private static void SetUpRates(IRateProviderService rateProviderService)
        {
            var rates = Enumerable
                .Range(0, 24)
                .Select(x => new { From = x, To = x + 1})
                .Select(x => new RateDefinition
                {
                    Days = "sun,mon,tues,wed,thurs,fri,sat",
                    Times = $"{x.From:D2}00-{x.To:D2}00",
                    Price = 40
                })
                .ToList();

            rateProviderService.UseRates(new RateDefinitions
            {
                Rates = rates
            });
        }

        private long GetRateQuotesPerformanceMilliseconds(IRateProviderService rateProviderService)
        {
            var everyMinuteInAWeek = Enumerable
                .Range(0, 60 * 24 * 7)
                .Select(x => TimeSpan.FromMinutes(x))
                .ToList();

            var sunday = new DateTimeOffset(2017, 8, 13, 0, 0, 0, TimeSpan.Zero);
            var ranges = everyMinuteInAWeek
                .Zip(everyMinuteInAWeek.Skip(60), (start, end) => new { From = sunday + start, To = sunday + end})
                .ToList();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 25; i++)
            {
                foreach (var range in ranges)
                {
                    rateProviderService.GetPrice(range.From, range.To);
                }
            }
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        [Fact]
        public void CompareLinearAndConstantTimeRateQuoteRetrieval()
        {
            var rateProviderServiceMilliseconds = GetRateQuotesPerformanceMilliseconds(_rateProviderService);
            var dictionaryRateProviderServiceMilliseconds = GetRateQuotesPerformanceMilliseconds(_dictionaryRateProviderService);

            _testOutputHelper.WriteLine($"{rateProviderServiceMilliseconds} ms for {nameof(RateProviderService)}");
            _testOutputHelper.WriteLine($"{dictionaryRateProviderServiceMilliseconds} ms for {nameof(DictionaryRateProviderService)}");
        }
    }
}
