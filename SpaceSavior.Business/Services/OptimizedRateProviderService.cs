using System;
using System.Collections.Generic;
using SpaceSavior.Business.Models;

namespace SpaceSavior.Business.Services
{
    public class OptimizedRateProviderService : IRateProviderService
    {
        private const int MinutesInAWeek = 60 * 24 * 7;

        private static readonly Dictionary<DayOfWeek, TimeSpan> DayOfWeekToTimespansWithThatDuration = new Dictionary<DayOfWeek, TimeSpan>
        {
            [DayOfWeek.Sunday] = new TimeSpan(0, 0, 0, 0),
            [DayOfWeek.Monday] = new TimeSpan(1, 0, 0, 0),
            [DayOfWeek.Tuesday] = new TimeSpan(2, 0, 0, 0),
            [DayOfWeek.Wednesday] = new TimeSpan(3, 0, 0, 0),
            [DayOfWeek.Thursday] = new TimeSpan(4, 0, 0, 0),
            [DayOfWeek.Friday] = new TimeSpan(5, 0, 0, 0),
            [DayOfWeek.Saturday] = new TimeSpan(6, 0, 0, 0)
        };

        private RateDateRange[] _rates;

        private readonly IRateConfigurationSettings _rateConfigurationSettings;

        public OptimizedRateProviderService(IRateConfigurationSettings rateConfigurationSettings)
        {
            _rateConfigurationSettings = rateConfigurationSettings;
        }

        public int? GetPrice(DateTimeOffset from, DateTimeOffset to)
        {
            if (from > to || from.DayOfWeek != to.DayOfWeek)
            {
                return null;
            }

            var fromMinutes = (int)(DayOfWeekToTimespansWithThatDuration[from.DayOfWeek] + from.TimeOfDay).TotalMinutes;

            var candidateRate = _rates[fromMinutes];

            if (candidateRate == null)
            {
                return null;
            }

            if (to.TimeOfDay < candidateRate.End)
            {
                return candidateRate.Price;
            }
            else
            {
                return null;
            }
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
            var rates = new RateDateRange[MinutesInAWeek];

            var rateDateRanges = rateSetInput.BuildRateDateRanges();
            foreach (var rateDateRange in rateDateRanges)
            {
                var start = DayOfWeekToTimespansWithThatDuration[rateDateRange.DayOfWeek] + rateDateRange.Start;
                var end = DayOfWeekToTimespansWithThatDuration[rateDateRange.DayOfWeek] + rateDateRange.End;

                for (int i = (int)start.TotalMinutes; i < (int)end.TotalMinutes; i++)
                {
                    if (rates[i] != null)
                    {
                        // If we're visiting a minute we've already processed, there's overlap in the rate definitions.
                        // Return null to indicate the rates are not valid.
                        return null;
                    }
                    rates[i] = rateDateRange;
                }
            }

            _rates = rates;
            return DateTimeOffset.UtcNow;
        }
    }
}
