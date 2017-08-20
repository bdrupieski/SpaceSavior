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

        private readonly OptimizedRateProviderService _optimizedRateProviderService;
        private readonly RateProviderService _rateProviderService;

        public RateProviderPerformanceComparisonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _optimizedRateProviderService = new OptimizedRateProviderService(new RateConfigurationSettings());
            _rateProviderService = new RateProviderService(new RateConfigurationSettings());

            SetUpRates(_optimizedRateProviderService);
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

        private static long GetRateQuotesPerformanceMilliseconds(IRateProviderService rateProviderService)
        {
            var everyMinuteInAWeek = Enumerable
                .Range(0, 60 * 24 * 7)
                .Select(x => TimeSpan.FromMinutes(x))
                .ToList();
            var everyMinuteInAWeekAnHourLater = everyMinuteInAWeek.Skip(60);

            var sunday = new DateTimeOffset(2017, 8, 13, 0, 0, 0, TimeSpan.Zero);
            var oneHourTimeIntervals = everyMinuteInAWeek
                .Zip(everyMinuteInAWeekAnHourLater, (start, end) => new { From = sunday + start, To = sunday + end })
                .ToList();

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 250; i++)
            {
                foreach (var timeInterval in oneHourTimeIntervals)
                {
                    rateProviderService.GetPrice(timeInterval.From, timeInterval.To);
                }
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        [Fact]
        public void CompareLinearAndConstantTimeRateQuoteRetrieval()
        {
            var rateProviderServiceMilliseconds = GetRateQuotesPerformanceMilliseconds(_rateProviderService);
            var optimizedRateProviderServiceMilliseconds = GetRateQuotesPerformanceMilliseconds(_optimizedRateProviderService);

            _testOutputHelper.WriteLine($"{rateProviderServiceMilliseconds} ms for {nameof(RateProviderService)}");
            _testOutputHelper.WriteLine($"{optimizedRateProviderServiceMilliseconds} ms for {nameof(OptimizedRateProviderService)}");

            double timesFaster = (rateProviderServiceMilliseconds - optimizedRateProviderServiceMilliseconds) / (double)optimizedRateProviderServiceMilliseconds;
            _testOutputHelper.WriteLine($"{nameof(OptimizedRateProviderService)} is {timesFaster:P2} faster than {nameof(RateProviderService)}");
        }
    }
}
