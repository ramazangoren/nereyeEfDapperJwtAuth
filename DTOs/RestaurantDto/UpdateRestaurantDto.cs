namespace api.DTOs.RestaurantDto
{
    public class UpdateRestaurantDto
    {
        public string RestaurantName { get; set; } = "";

        // public int? RestaurantCode { get; set; }
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
        public string MondayOpens { get; set; } = "";
        public string MondayCloses { get; set; } = "";
        public string TuesdayOpens { get; set; } = "";
        public string TuesdayCloses { get; set; } = "";
        public string WednesdayOpens { get; set; } = "";
        public string WednesdayCloses { get; set; } = "";
        public string ThursdayOpens { get; set; } = "";
        public string ThursdayCloses { get; set; } = "";
        public string FridayOpens { get; set; } = "";
        public string FridayCloses { get; set; } = "";
        public string SaturdayOpens { get; set; } = "";
        public string SaturdayCloses { get; set; } = "";
        public string SundayOpens { get; set; } = "";
        public string SundayCloses { get; set; } = "";
        public bool Active { get; set; } = true;
    }
}
