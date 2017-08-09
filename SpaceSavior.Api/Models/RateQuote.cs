using Newtonsoft.Json;

namespace SpaceSavior.Api.Models
{
    /// <summary>
    /// Represents a quoted rate price for a given datetime range.
    /// </summary>
    public class RateQuote
    {
        /// <summary>
        /// An echo of the requested start of the rate date range.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// An echo of the requested end of the rate date range.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// If available, the rate for the requested date range.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Price { get; set; }

        /// <summary>
        /// Indicates if a price is available for the date range 
        /// formed by the requested <see cref="From"/> and <see cref="To"/>.
        /// </summary>
        public bool Available { get; set; }
    }
}
