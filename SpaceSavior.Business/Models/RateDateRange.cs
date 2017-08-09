using System;

namespace SpaceSavior.Business.Models
{
    public class RateDateRange
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Price { get; set; }
    }
}