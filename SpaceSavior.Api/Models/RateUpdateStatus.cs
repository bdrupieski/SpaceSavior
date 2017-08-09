using System;
using Newtonsoft.Json;

namespace SpaceSavior.Api.Models
{
    /// <summary>
    /// Represents the completion status of a request to update the rate definitions used by the API.
    /// </summary>
    public class RateUpdateStatus
    {
        /// <summary>
        /// True if the rates were changed by this request.
        /// </summary>
        public bool RatesSuccessfullyChanged { get; set; }

        /// <summary>
        /// The approximate time the rates were changed. This may
        /// be useful to know when there are many requests for rate quotes and
        /// you need to know exactly when the rates switched.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? ApproximateDateTimeRatesChanged { get; set; }
    }
}
