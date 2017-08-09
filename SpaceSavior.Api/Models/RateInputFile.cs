using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SpaceSavior.Business.Models;

namespace SpaceSavior.Api.Models
{
    /// <summary>
    /// Represents a file containing a collection of rate definitions 
    /// to be used as the basis for producing rate quotes.
    /// </summary>
    public class RateInputFile
    {
        /// <summary>
        /// The collection of rate definitions in the rate definition input file.
        /// </summary>
        [JsonRequired]
        public List<RateInput> Rates { get; set; }

        public RateDefinitions MapToDefinitions()
        {
            return new RateDefinitions
            {
                Rates = Rates.Select(x => new RateDefinition
                    {
                        Times = x.Times,
                        Days = x.Days,
                        Price = x.Price,
                    })
                    .ToList()
            };
        }
    }
}
