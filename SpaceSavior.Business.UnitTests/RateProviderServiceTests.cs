using System;
using System.Collections.Generic;
using SpaceSavior.Business.Models;
using SpaceSavior.Business.Services;
using Xunit;

namespace SpaceSavior.Business.UnitTests
{
    /// <summary>
    /// The test names in this class follow the convention
    /// MethodName_StateUnderTest_ExpectedBehavior
    /// </summary>
    public class RateProviderServiceTests
    {
        private readonly IRateProviderService _rateProviderService;

        public RateProviderServiceTests()
        {
            _rateProviderService = new RateProviderService(new RateConfigurationSettings());
        }

        [Fact]
        public void GetPrice_InclusiveAtStartOfRangeExclusiveAtEndOfRangeForSingleRange_RetrievesCorrectPrice()
        {
            int expectedPrice = 2000;
            _rateProviderService.UseRates(new RateDefinitions
            {
                Rates = new List<RateDefinition>
                {
                    new RateDefinition
                    {
                        Days = "sun,mon",
                        Times = "0600-1800",
                        Price = expectedPrice
                    },
                }
            });

            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Monday(06, 00), Monday(07, 00)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Monday(07, 00), Monday(08, 00)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Monday(07, 00), Monday(17, 59)));
            Assert.Equal(null, _rateProviderService.GetPrice(Monday(07, 00), Monday(18, 00)));
        }

        [Fact]
        public void GetPrice_InclusiveAtStartOfRangeExclusiveAtEndOfRangeForMultipleRanges_RetrievesCorrectPrice()
        {
            _rateProviderService.UseRates(new RateDefinitions
            {
                Rates = new List<RateDefinition>
                {
                    new RateDefinition
                    {
                        Days = "mon",
                        Times = "0600-0700",
                        Price = 10
                    },
                    new RateDefinition
                    {
                        Days = "mon",
                        Times = "0700-0800",
                        Price = 20
                    },
                    new RateDefinition
                    {
                        Days = "mon",
                        Times = "0800-0900",
                        Price = 30
                    },
                }
            });

            Assert.Equal(10, _rateProviderService.GetPrice(Monday(06, 00), Monday(06, 59)));
            Assert.Equal(20, _rateProviderService.GetPrice(Monday(07, 00), Monday(07, 10)));
            Assert.Equal(20, _rateProviderService.GetPrice(Monday(07, 10), Monday(07, 50)));
            Assert.Equal(null, _rateProviderService.GetPrice(Monday(07, 10), Monday(08, 00)));
            Assert.Equal(30, _rateProviderService.GetPrice(Monday(08, 00), Monday(08, 10)));
        }

        [Fact]
        public void GetPrice_DayOfWeekRespected_RetrievesCorrectPrice()
        {
            int expectedPrice = 2000;
            _rateProviderService.UseRates(new RateDefinitions
            {
                Rates = new List<RateDefinition>
                {
                    new RateDefinition
                    {
                        Days = "sun,mon",
                        Times = "0600-1800",
                        Price = expectedPrice
                    },
                }
            });

            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Monday(10, 00), Monday(10, 10)));
            Assert.Equal(null, _rateProviderService.GetPrice(Monday(10, 00), Tuesday(10, 10)));
        }

        [Fact]
        public void GetPrice_AllDaysOfTheWeek_RetrievesCorrectPrice()
        {
            int expectedPrice = 2000;
            _rateProviderService.UseRates(new RateDefinitions
            {
                Rates = new List<RateDefinition>
                {
                    new RateDefinition
                    {
                        Days = "sun,mon,tues,wed,thurs,fri,sat",
                        Times = "0600-1800",
                        Price = expectedPrice
                    },
                }
            });

            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Sunday(10, 00), Sunday(10, 10)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Monday(10, 00), Monday(10, 10)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Tuesday(10, 00), Tuesday(10, 10)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Wednesday(10, 00), Wednesday(10, 10)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Thursday(10, 00), Thursday(10, 10)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Friday(10, 00), Friday(10, 10)));
            Assert.Equal(expectedPrice, _rateProviderService.GetPrice(Saturday(10, 00), Saturday(10, 10)));
        }

        [Fact]
        public void GetPrice_ContinuousRanges_RetrievesCorrectPrice()
        {
            _rateProviderService.UseRates(new RateDefinitions
            {
                Rates = new List<RateDefinition>
                {
                    new RateDefinition
                    {
                        Days = "sun,mon,tues,wed",
                        Times = "0600-0700",
                        Price = 10
                    },
                    new RateDefinition
                    {
                        Days = "sun,mon,tues,wed",
                        Times = "0700-0800",
                        Price = 20
                    },
                    new RateDefinition
                    {
                        Days = "sun,mon,tues,wed",
                        Times = "0800-0900",
                        Price = 30
                    },
                    new RateDefinition
                    {
                        Days = "sun,mon,tues,wed",
                        Times = "0900-1000",
                        Price = 40
                    },
                }
            });

            Assert.Equal(null, _rateProviderService.GetPrice(Monday(05, 30), Monday(05, 40)));
            Assert.Equal(10, _rateProviderService.GetPrice(Monday(06, 30), Monday(06, 40)));
            Assert.Equal(20, _rateProviderService.GetPrice(Monday(07, 30), Monday(07, 40)));
            Assert.Equal(30, _rateProviderService.GetPrice(Monday(08, 30), Monday(08, 40)));
            Assert.Equal(40, _rateProviderService.GetPrice(Monday(09, 30), Monday(09, 40)));
            Assert.Equal(null, _rateProviderService.GetPrice(Monday(10, 30), Monday(10, 40)));
        }

        private DateTimeOffset Sunday(int hour, int minute) => new DateTimeOffset(2017, 7, 30, hour, minute, 0, TimeSpan.Zero);
        private DateTimeOffset Monday(int hour, int minute) => new DateTimeOffset(2017, 7, 31, hour, minute, 0, TimeSpan.Zero);
        private DateTimeOffset Tuesday(int hour, int minute) => new DateTimeOffset(2017, 8, 1, hour, minute, 0, TimeSpan.Zero);
        private DateTimeOffset Wednesday(int hour, int minute) => new DateTimeOffset(2017, 8, 2, hour, minute, 0, TimeSpan.Zero);
        private DateTimeOffset Thursday(int hour, int minute) => new DateTimeOffset(2017, 8, 3, hour, minute, 0, TimeSpan.Zero);
        private DateTimeOffset Friday(int hour, int minute) => new DateTimeOffset(2017, 8, 4, hour, minute, 0, TimeSpan.Zero);
        private DateTimeOffset Saturday(int hour, int minute) => new DateTimeOffset(2017, 8, 5, hour, minute, 0, TimeSpan.Zero);
    }
}
