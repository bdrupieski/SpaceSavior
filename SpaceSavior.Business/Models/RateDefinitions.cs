using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SpaceSavior.Business.Models
{
    /// <summary>
    /// DTO for JSON deserialization of a set of rate definitions.
    /// </summary>
    public class RateDefinitions
    {
        const string RateTimespanNotInCorrectFormatMessage = "Rate timespan is not in the expected format.";

        public List<RateDefinition> Rates { get; set; }

        public static RateDefinitions ParseFromJsonFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<RateDefinitions>(json);
        }

        public IEnumerable<RateDateRange> BuildRateDateRanges()
        {
            var rateRange = new List<RateDateRange>();
            foreach (var rawRate in Rates)
            {
                var daysOfWeek = rawRate.Days.Split(',');
                var startAndEndTime = rawRate.Times.Split('-');

                if (startAndEndTime.Length != 2)
                {
                    throw new ArgumentException($"{RateTimespanNotInCorrectFormatMessage} Instead of a string with a dash, it's {rawRate.Times}.", nameof(rawRate.Times));
                }

                string startTime = startAndEndTime[0];
                string endTime = startAndEndTime[1];

                foreach (var dayOfWeek in daysOfWeek)
                {
                    var rateDateRange = new RateDateRange
                    {
                        DayOfWeek = ParseDayOfWeek(dayOfWeek),
                        Start = Parse24HourClockAsTimeSpan(startTime),
                        End = Parse24HourClockAsTimeSpan(endTime),
                        Price = rawRate.Price,
                    };
                    rateRange.Add(rateDateRange);
                }
            }
            return rateRange;
        }

        private static TimeSpan Parse24HourClockAsTimeSpan(string time)
        {
            if (time.Length != 4)
            {
                throw new ArgumentException($"{RateTimespanNotInCorrectFormatMessage} Instead of four characters, it's {time}.", nameof(time));
            }

            if (!int.TryParse(time.Substring(0, 2), out var hours) || !int.TryParse(time.Substring(2, 2), out var minutes))
            {
                throw new ArgumentException($"{RateTimespanNotInCorrectFormatMessage} Instead of two numbers, it's {time}.", nameof(time));
            }

            return new TimeSpan(days: 0, hours: hours, minutes: minutes, seconds: 0);
        }

        private static DayOfWeek ParseDayOfWeek(string dayOfWeek)
        {
            switch (dayOfWeek.ToLower())
            {
                case "sun":
                    return DayOfWeek.Sunday;
                case "mon":
                    return DayOfWeek.Monday;
                case "tues":
                    return DayOfWeek.Tuesday;
                case "wed":
                    return DayOfWeek.Wednesday;
                case "thurs":
                    return DayOfWeek.Thursday;
                case "fri":
                    return DayOfWeek.Friday;
                case "sat":
                    return DayOfWeek.Saturday;
                default:
                    throw new ArgumentException($"{dayOfWeek} is not a valid day of the week.", nameof(dayOfWeek));
            }
        }
    }
}
