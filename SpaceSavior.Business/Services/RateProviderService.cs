using System;
using System.Collections.Generic;
using System.Linq;
using SpaceSavior.Business.Models;

namespace SpaceSavior.Business.Services
{
    public class RateProviderService : IRateProviderService
    {
        private readonly IRateConfigurationSettings _rateConfigurationSettings;

        private Dictionary<DayOfWeek, List<RateDateRange>> _rateDateRangesByDayOfWeek;

        public RateProviderService(IRateConfigurationSettings rateConfigurationSettings)
        {
            _rateConfigurationSettings = rateConfigurationSettings;
        }

        public int? GetPrice(DateTimeOffset from, DateTimeOffset to)
        {
            var fromUtc = from.ToUniversalTime();
            var toUtc = to.ToUniversalTime();

            if (fromUtc.DayOfWeek != toUtc.DayOfWeek ||
                fromUtc > toUtc)
            {
                return null;
            }

            DayOfWeek dayOfWeek = fromUtc.DayOfWeek;
            if (!_rateDateRangesByDayOfWeek.TryGetValue(dayOfWeek, out var ratesForDay))
            {
                return null;
            }

            var candidateRate = ratesForDay.FirstOrDefault(x => fromUtc.TimeOfDay >= x.Start && toUtc.TimeOfDay < x.End);
            return candidateRate?.Price;
        }

        public bool SetUpConfiguredDefaultRates()
        {
            var rateFileInput = RateDefinitions.ParseFromJsonFile(_rateConfigurationSettings.DefaultRatesInputFilePath);
            var timeRatesChanged = UseRates(rateFileInput);
            bool success = timeRatesChanged != null;

            return success;
        }

        public DateTimeOffset? UseRates(RateDefinitions rateSetInput)
        {
            var rateDateRanges = rateSetInput
                .BuildRateDateRanges()
                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, y => y.OrderBy(x => x.Start).ToList());

            DateTimeOffset? timeRatesChanged = null;

            if (AreDateRangesValid(rateDateRanges))
            {
                _rateDateRangesByDayOfWeek = rateDateRanges;
                timeRatesChanged = DateTimeOffset.UtcNow;
            }

            return timeRatesChanged;
        }

        private static bool AreDateRangesValid(Dictionary<DayOfWeek, List<RateDateRange>> rateDateRanges)
        {
            foreach (var ratesForDay in rateDateRanges)
            {
                var ratesOrderedByStartTime = ratesForDay.Value.OrderBy(x => x.Start);
                var pairsOfRatesMovingForwardInTime = ratesOrderedByStartTime.Zip(ratesOrderedByStartTime.Skip(1), (a, b) => (a, b));
                foreach (var (rateA, rateB) in pairsOfRatesMovingForwardInTime)
                {
                    if (rateA.End > rateB.Start)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
