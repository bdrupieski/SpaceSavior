using Newtonsoft.Json;

namespace SpaceSavior.Api.Models
{
    /// <summary>
    /// Represents a single rate definition, containing the times the rate is effective.
    /// </summary>
    public class RateInput
    {
        /// <summary>
        /// A comma-separated collection of strings representing days of the week.
        /// Valid values include "sun", "mon", "tues", "wed", "thurs", "fri", and "sat".
        /// For example, "sun,mon,tues".
        /// </summary>
        [JsonRequired]
        public string Days { get; set; }

        /// <summary>
        /// A time range using a 24-hour clock of hours and minutes, with the two times separated by a single dash.
        /// For example, "0615-0700" to indicate 6:15 AM to 7:00 AM for a given day.
        /// </summary>
        [JsonRequired]
        public string Times { get; set; }

        /// <summary>
        /// An integer representing the price in cents of the given rate definition.
        /// </summary>
        [JsonRequired]
        public int Price { get; set; }
    }
}
