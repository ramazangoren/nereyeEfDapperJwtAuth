using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.RestaurantDto
{
    public class RestaurantFilter
    {
        public string? RestaurantName { get; set; }
        public string? EstablishmentType { get; set; }
        public bool? CreditCard { get; set; }
        public bool? Cash { get; set; }
        public bool? FoodCards { get; set; }
        public bool? AppleGooglePay { get; set; }
        public bool? OutdoorSitting { get; set; }
        public bool? PetsAllowed { get; set; }
        public bool? Alcohol { get; set; }
        public bool? Parking { get; set; }
        public bool? OffersDelivery { get; set; }
        public bool? OffersTakeout { get; set; }
        public bool? GoodForGroups { get; set; }
        public bool? GoodForKids { get; set; }
        public bool? FullBar { get; set; }
        public bool? TakesReservation { get; set; }
        public bool? WaiterService { get; set; }
        public bool? SelfService { get; set; }
        public bool? HasTV { get; set; }
        public bool? FreeWifi { get; set; }
        public bool? StreetParking { get; set; }
        public bool? BeerAndWineOnly { get; set; }
        public bool? Italian { get; set; }
        public bool? Hookah { get; set; }
        public bool? Burger { get; set; }
        public bool? HotDogs { get; set; }
        public bool? FastFood { get; set; }
        public bool? Breakfast { get; set; }
        public bool? Doner { get; set; }
        public bool? HalalFood { get; set; }
        public int? Rating { get; set; }
        public bool? OpenStatus { get; set; }
    }
}
