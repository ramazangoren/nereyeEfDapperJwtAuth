using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public int UserId { get; set; }
        public string RestaurantName { get; set; } = "";
        public int? RestaurantCode { get; set; }
        public string FullAddress { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";
        public string EstablishmentType { get; set; } = "";
        public string Phone { get; set; } = "";
        public string ExtraInfo { get; set; } = "";
        public string AboutUs { get; set; } = "";
        public bool CreditCard { get; set; }
        public bool Cash { get; set; }
        public bool FoodCards { get; set; }
        public bool AppleGooglePay { get; set; }
        public bool OutdoorSitting { get; set; }
        public bool PetsAllowed { get; set; }
        public bool Alcohol { get; set; }
        public bool Parking { get; set; }
        public bool OffersDelivery { get; set; }
        public bool OffersTakeout { get; set; }
        public bool GoodForGroups { get; set; }
        public bool GoodForKids { get; set; }
        public bool FullBar { get; set; }
        public bool TakesReservation { get; set; }
        public bool WaiterService { get; set; }
        public bool SelfService { get; set; }
        public bool HasTV { get; set; }
        public bool FreeWifi { get; set; }
        public bool StreetParking { get; set; }
        public bool BeerAndWineOnly { get; set; }
        public bool Italian { get; set; }
        public bool Hookah { get; set; }
        public bool Burger { get; set; }
        public bool HotDogs { get; set; }
        public bool FastFood { get; set; }
        public bool Breakfast { get; set; }
        public bool Doner { get; set; }
        public bool HalalFood { get; set; }
        public string ImageUrls { get; set; } = "";
        public TimeSpan MondayOpens { get; set; } = TimeSpan.Zero;
        public TimeSpan MondayCloses { get; set; } = TimeSpan.Zero;
        public TimeSpan TuesdayOpens { get; set; } = TimeSpan.Zero;
        public TimeSpan TuesdayCloses { get; set; } = TimeSpan.Zero;
        public TimeSpan WednesdayOpens { get; set; } = TimeSpan.Zero;
        public TimeSpan WednesdayCloses { get; set; } = TimeSpan.Zero;
        public TimeSpan ThursdayOpens { get; set; } = TimeSpan.Zero;
        public TimeSpan ThursdayCloses { get; set; } = TimeSpan.Zero;
        public TimeSpan FridayOpens { get; set; } = TimeSpan.Zero;
        public TimeSpan FridayCloses { get; set; } = TimeSpan.Zero;
        public TimeSpan SaturdayOpens { get; set; } = TimeSpan.Zero;
        public TimeSpan SaturdayCloses { get; set; } = TimeSpan.Zero;
        public TimeSpan SundayOpens { get; set; } = TimeSpan.Zero;
        public TimeSpan SundayCloses { get; set; } = TimeSpan.Zero;

        // Timestamp fields
        // [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public bool Active { get; set; } = true;
        public bool? OpenStatus { get; set; }
    }
}
