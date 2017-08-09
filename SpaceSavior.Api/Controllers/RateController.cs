using System;
using Microsoft.AspNetCore.Mvc;
using SpaceSavior.Api.Models;
using SpaceSavior.Business.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SpaceSavior.Api.Controllers
{
    public class RateController : Controller
    {
        private readonly IRateProviderService _rateProviderService;

        public RateController(IRateProviderService rateProviderService)
        {
            _rateProviderService = rateProviderService;
        }

        /// <summary>
        /// Retrieves a rate quote for a given datetime range formed by two datetimes.
        /// </summary>
        /// <remarks>
        /// Rates are retrieved from an in-memory representation of the rates. 
        /// This endpoint will not scale to multiple instances.
        /// </remarks>
        /// <response code="200">
        /// Object representing a rate quote, including a price if available and 
        /// a flag to indicate if a price is not available for the requested daterange.
        /// </response>
        /// <response code="400">
        /// Bad Request - This can occur if:
        /// - The from and to parameters are missing
        /// - The from and to parameters are not valid datetimes
        /// - The from datetime is greater than the to datetime
        /// </response>
        /// <param name="from">The start of the date range to get a rate quote for.</param>
        /// <param name="to">The end of the date range to get a rate quote for.</param>
        [HttpGet]
        [Route("v1/ratequote/from/{from}/to/{to}")]
        [SwaggerResponse(200, typeof(RateQuote))]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        public IActionResult GetRateQuote(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return BadRequest();
            }

            if (!DateTimeOffset.TryParse(from, out var fromTime) || !DateTimeOffset.TryParse(to, out var toTime))
            {
                return BadRequest();
            }

            if (fromTime > toTime)
            {
                return BadRequest();
            }

            var price = _rateProviderService.GetPrice(fromTime, toTime);

            return Ok(new RateQuote
            {
                From = from,
                To = to,
                Price = price,
                Available = price != null,
            });
        }

        /// <summary>
        /// Updates the set of rate definitions to use when determining rate quotes.
        /// </summary>
        /// <remarks>
        /// Rates are stored in-memory per instance. Updating the rates
        /// using this endpoint will only update the rates for this instance.
        /// Other instances will not be affected.
        /// </remarks>
        /// <response code="200">
        /// Object representing an indication of whether or not the rates 
        /// were updated, and an approximate time of when the change occurred.
        /// </response>
        /// <response code="400">
        /// Bad Request - This can occur if:
        /// - The input JSON is malformed
        /// - The input set of rates contains zero rate definitions
        /// </response>
        /// <param name="rateInputFile">JSON input of a set of rate definitions</param>
        [HttpPost]
        [Route("v1/rates")]
        [SwaggerResponse(200, typeof(RateUpdateStatus))]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        public IActionResult UpdateRates([FromBody] RateInputFile rateInputFile)
        {
            if (rateInputFile.Rates.Count == 0)
            {
                return BadRequest();
            }

            var dateTimeRatesChanged = _rateProviderService.UseRates(rateInputFile.MapToDefinitions());

            return Ok(new RateUpdateStatus
            {
                RatesSuccessfullyChanged = dateTimeRatesChanged != null,
                ApproximateDateTimeRatesChanged = dateTimeRatesChanged,
            });
        }
    }
}
