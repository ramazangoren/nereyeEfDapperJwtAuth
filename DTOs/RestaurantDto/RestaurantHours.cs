using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.RestaurantDto
{
    public class RestaurantHours
    {
        public TimeSpan MondayOpens { get; set; }
        public TimeSpan MondayCloses { get; set; }
        public TimeSpan TuesdayOpens { get; set; }
        public TimeSpan TuesdayCloses { get; set; }
        public TimeSpan WednesdayOpens { get; set; }
        public TimeSpan WednesdayCloses { get; set; }
        public TimeSpan ThursdayOpens { get; set; }
        public TimeSpan ThursdayCloses { get; set; }
        public TimeSpan FridayOpens { get; set; }
        public TimeSpan FridayCloses { get; set; }
        public TimeSpan SaturdayOpens { get; set; }
        public TimeSpan SaturdayCloses { get; set; }
        public TimeSpan SundayOpens { get; set; }
        public TimeSpan SundayCloses { get; set; }
    }
}
